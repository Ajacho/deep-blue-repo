using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Uxcheckmate_Main.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;


namespace Uxcheckmate_Main.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReportService> _logger;
        private readonly UxCheckmateDbContext _dbContext;
        private readonly IOpenAiService _openAiService;
        private readonly IBrokenLinksService _brokenLinksService;
        private readonly IHeadingHierarchyService _headingHierarchyService;
        private readonly IColorSchemeService _colorSchemeService;
        private readonly IMobileResponsivenessService _mobileResponsivenessService;
      //  private readonly IWebScraperService _scraperService;
        private readonly IScreenshotService _screenshotService;
        private readonly IPlaywrightScraperService _playwrightScraperService;
        private readonly IPopUpsService _popUpsService;
        private readonly IAnimationService _animationService;
        private readonly IAudioService _audioService;
        private readonly IScrollService _scrollService;


        public ReportService(HttpClient httpClient, ILogger<ReportService> logger, UxCheckmateDbContext context, IOpenAiService openAiService, IBrokenLinksService brokenLinksService, IHeadingHierarchyService headingHierarchyService, IColorSchemeService colorSchemeService, IMobileResponsivenessService mobileResponsivenessService, IScreenshotService screenshotService, IPlaywrightScraperService playwrightScraperService, IPopUpsService popUpsService, IAnimationService animationService, IAudioService audioService, IScrollService scrollService)
        {
            _httpClient = httpClient;
            _dbContext = context;
            _openAiService = openAiService;
            _logger = logger;
            _brokenLinksService = brokenLinksService;
            _headingHierarchyService = headingHierarchyService;
            _colorSchemeService = colorSchemeService;
            _mobileResponsivenessService = mobileResponsivenessService;
            _screenshotService = screenshotService;
          //  _scraperService = scraperService;
            _playwrightScraperService = playwrightScraperService;
            _popUpsService = popUpsService;
            _animationService = animationService;
            _audioService = audioService;
            _scrollService = scrollService;
        }


        public async Task<ICollection<DesignIssue>> GenerateReportAsync(Report report)
        {
            // Initialize url to report attribute
            var url = report.Url;
            _logger.LogInformation("Starting report generation for URL: {Url}", url);


            // If there is no url throw an exception
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogError("URL is null or empty.");
                throw new ArgumentException("URL cannot be empty.", nameof(url));
            }
            
            // Scrape site
            var fullScraped = await _playwrightScraperService.ScrapeEverythingAsync(url);
            var scrapedData = fullScraped.ToDictionary();

            // Call the scrapers in parallel
          //  var staticScrapeTask = _scraperService.ScrapeAsync(url);
           // var dynamicScrapeTask = _playwrightScraperService.ScrapeAsync(url);

           // await Task.WhenAll(staticScrapeTask, dynamicScrapeTask);

           // var scrapedData = staticScrapeTask.Result;
           // var assets = dynamicScrapeTask.Result;

            // Merge Static and Dynamic content into one dictionary
           // scrapedData = MergeScrapedData(scrapedData, assets);

           /* foreach (var css in assets.ExternalCssContents)
            {
                _logger.LogInformation("External CSS Content Preview:\n{Css}", css.Substring(0, Math.Min(200, css.Length)));
            }

            foreach (var js in assets.ExternalJsContents)
            {
                _logger.LogInformation("External JS Content Preview:\n{Js}", js.Substring(0, Math.Min(200, js.Length)));
            }*/

            // Get list of design categories
            var designCategories = await _dbContext.DesignCategories.ToListAsync();
            _logger.LogInformation("Found {Count} design categories.", designCategories.Count);

            // Use ConcurrentBag for thread-safe collection of results
            var scanResults = new ConcurrentBag<DesignIssue>();

            // Run analysis for each category in parallel
            await Parallel.ForEachAsync(
                designCategories,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 },
                async (category, cancellationToken) =>
                {
                    _logger.LogInformation("Analyzing category: {CategoryName} using scan method: {ScanMethod}", category.Name, category.ScanMethod);

                    string message;
                    try
                    {
                        message = category.ScanMethod switch
                        {
                            "OpenAI" => await _openAiService.AnalyzeWithOpenAI(url, category.Name, category.Description, scrapedData),
                            "Custom" => await RunCustomAnalysisAsync(url, category.Name, category.Description, scrapedData),
                            _ => ""
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error analyzing category {CategoryName}: {ErrorMessage}", category.Name, ex.Message);
                        message = "";
                    }

                    // Connect service response to dbset attributes
                    if (!string.IsNullOrEmpty(message))
                    {
                        var designIssue = new DesignIssue
                        {
                            CategoryId = category.Id,
                            ReportId = report.Id,
                            Message = message,
                            Severity = DetermineSeverity(message)
                        };

                        // Add to results collection
                        scanResults.Add(designIssue);
                        _logger.LogInformation("Design issue added for category {CategoryName} with severity {Severity}.", category.Name, designIssue.Severity);
                    }
                    else
                    {
                        _logger.LogInformation("No issues found for category: {CategoryName}", category.Name);
                    }
                });
            if (report.Id > 0){
                // Add all design issues to the context
                _dbContext.DesignIssues.AddRange(scanResults);
                // Save to database
                await _dbContext.SaveChangesAsync();
            }else{
                _logger.LogInformation("Skipping saving DesignIssues");    
            }
            // Return report
            return scanResults.ToList();
        }

        public async Task<string> RunCustomAnalysisAsync(string url, string categoryName, string categoryDescription, Dictionary<string, object> scrapedData)
        {
            _logger.LogInformation("Running custom analysis for category: {CategoryName}", categoryName);
            Task<byte[]> screenshotTask = _screenshotService?.CaptureFullPageScreenshot(url) ?? Task.FromResult(new byte[0]); // Full site screenshot for anaylsis

            switch (categoryName)
            {
                case "Broken Links":
                    return await _brokenLinksService.BrokenLinkAnalysis(url, scrapedData);

                case "Visual Hierarchy":
                    return await _headingHierarchyService.AnalyzeAsync(scrapedData);

                case "Color Scheme":
                    return await _colorSchemeService.AnalyzeWebsiteColorsAsync(scrapedData, screenshotTask);

                case "Mobile Responsiveness":
                    return await _mobileResponsivenessService.RunMobileAnalysisAsync(url, scrapedData);

                case "Favicon":
                    return await AnalyzeFaviconAsync(url, scrapedData);

                case "Font Legibility":
                    return await AnalyzeFontLegibilityAsync(url, scrapedData);
               
                case "Pop Ups":
                    return await _popUpsService.RunPopupAnalysisAsync(url, scrapedData);

                case "Animations":
                    return await _animationService.RunAnimationAnalysisAsync(url, scrapedData);

                case "Audio":
                    return await _audioService.RunAudioAnalysisAsync(url, scrapedData);

                case "Number of scrolls":
                    return await _scrollService.RunScrollAnalysisAsync(url, scrapedData);

                default:
                    _logger.LogDebug("No custom analysis implemented for category: {CategoryName}", categoryName);
                    return string.Empty;
            }
        }

     /*   private async Task<string> AnalyzeDynamicSizingAsync(string url, Dictionary<string, object> scrapedData)
        {
            bool hasDynamicSizing = _MobileResponsivenessIMobileResponsivenessService.HasDynamicSizing(scrapedData["htmlContent"].ToString());

            if (!hasDynamicSizing)
            {
                string prompt = $"The website at {url} is missing dynamic sizing elements (e.g., media queries, viewport meta tag, flexbox/grid layout). Please provide a recommendation on how to implement dynamic sizing.";
                return await _openAiService.AnalyzeWithOpenAI(url, "Dynamic Sizing", prompt, scrapedData);
            }

            _logger.LogDebug("Dynamic sizing elements are present. No recommendations needed.");
            return string.Empty;
        }*/

        private async Task<string> AnalyzeFaviconAsync(string url, Dictionary<string, object> scrapedData)
        {
            bool hasFavicon = scrapedData.TryGetValue("hasFavicon", out var hasFaviconObj) && hasFaviconObj is bool value && value;

            if (!hasFavicon)
            {
                _logger.LogWarning("❌ No favicon detected for URL: {Url}", url);
                return "No favicon found on this website. Consider adding a favicon for better branding and user recognition.";
            }

            _logger.LogInformation("✅ Favicon detected for URL: {Url}", url);
            return string.Empty;
        }

        private async Task<string> AnalyzeFontLegibilityAsync(string url, Dictionary<string, object> scrapedData)
        {
            if (scrapedData.TryGetValue("fonts", out var fontsObj) && fontsObj is List<string> fontsUsed)
            {
                _logger.LogInformation("Extracted fonts: {@Fonts}", fontsUsed); // Log extracted fonts

                // Normalize both lists to lowercase for case-insensitive comparison
                var illegibleFontsNormalized = FontLegibilityModel.IllegibleFonts
                    .Select(f => f.ToLowerInvariant().Trim())
                    .ToList();

                var fontsUsedNormalized = fontsUsed
                    .Select(f => f.ToLowerInvariant().Trim().Trim('"'))
                    .ToList();

                _logger.LogInformation("Normalized Illegible Fonts: {@IllegibleFonts}", illegibleFontsNormalized);
                _logger.LogInformation("Normalized Extracted Fonts: {@FontsUsed}", fontsUsedNormalized);

                // Debug log: Check EXACTLY what's in both lists
                _logger.LogInformation("Comparing extracted fonts with illegible fonts...");

                foreach (var font in fontsUsedNormalized)
                {
                    if (illegibleFontsNormalized.Contains(font))
                    {
                        _logger.LogInformation("Match found: {Font} is illegible!", font);
                    }
                }

                var illegibleFontsFound = fontsUsedNormalized.Intersect(illegibleFontsNormalized).ToList();

                if (illegibleFontsFound.Any())
                {
                    string issueMessage = $"The following fonts are considered illegible: {string.Join(", ", illegibleFontsFound)}. Consider using more readable fonts for better accessibility.";
                    _logger.LogInformation("Issue detected: {IssueMessage}", issueMessage);
                    return issueMessage;
                }
                else
                {
                    _logger.LogInformation("✅ No illegible fonts detected for URL: {Url}", url);
                    return string.Empty;
                }
            }
            else
            {
                _logger.LogWarning("❌ Font extraction failed or no fonts found for URL: {Url}", url);
                return "No fonts were detected on this website. Ensure that text elements specify a font-family.";
            }
        }

        private int DetermineSeverity(string aiText)
        {
            _logger.LogDebug("Determining severity for analysis text.");
            return aiText.Contains("critical", StringComparison.OrdinalIgnoreCase) ? 3 :
                   aiText.Contains("should", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
        }
    /*    private Dictionary<string, object> MergeScrapedData(Dictionary<string, object> baseData, ScrapedContent assets)
        {
            baseData["url"] = assets.Url;
            baseData["html"] = assets.Html;

            baseData["inlineCss"] = assets.InlineCss;
            baseData["inlineJs"] = assets.InlineJs;

            baseData["inlineCssList"] = assets.InlineCssList;
            baseData["inlineJsList"] = assets.InlineJsList;

            baseData["externalCssLinks"] = assets.ExternalCssLinks;
            baseData["externalJsLinks"] = assets.ExternalJsLinks;

            baseData["externalCssContents"] = assets.ExternalCssContents;
            baseData["externalJsContents"] = assets.ExternalJsContents;

            baseData["scrollHeight"] = assets.ScrollHeight;
            baseData["viewportHeight"] = assets.ViewportHeight;

            baseData["scrollWidth"] = assets.ScrollWidth;
            baseData["viewportWidth"] = assets.ViewportWidth;
            baseData["viewportLabel"] = assets.ViewportLabel;

            return baseData;
        }*/


    }
}