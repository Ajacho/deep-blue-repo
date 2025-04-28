using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Uxcheckmate_Main.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Uxcheckmate_Main.Services
{
    public class PlaywrightScraperService : IPlaywrightScraperService
    {
        private readonly IPlaywrightService _playwrightService;
        private readonly ILogger<PlaywrightScraperService> _logger;
        private readonly ILayoutParsingService _layoutParsingService;

        public PlaywrightScraperService(IPlaywrightService playwrightService, ILogger<PlaywrightScraperService> logger, ILayoutParsingService layoutParsingService)
        {
            _playwrightService = playwrightService;
            _logger = logger;
            _layoutParsingService = layoutParsingService;
        }

        public async Task<ScrapedContent> ScrapeEverythingAsync(string url)
        {
            // Get a browser instance from the Playwright service (already launched)
            var browser = await _playwrightService.GetBrowserAsync();

            // Create a new isolated browser context (clears cookies, sessions)
            // var context = await browser.NewContextAsync();
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                BypassCSP = true, // Bypass Content Security Policy
            });


            // Open a new page (tab) within the browser context
            var page = await context.NewPageAsync();

            try
            {
                // Navigate to the target URL and wait until all network requests are finished
                await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                // Inject custom helper script for scraping external CSS and JS
                await page.AddScriptTagAsync(new PageAddScriptTagOptions
                {
                    Path = "wwwroot/js/playwrightScraperHelper.js"
                });

                // Run a client-side Script in the page context to extract all DOM-based metadata
                // Return it as a plain JS object and deserialize as a JsonElement for parsing
                var rawData = await page.EvaluateAsync<JsonElement>("() => window.scrapeDomData()");

                // Ensure the returned data is a valid object
                if (rawData.ValueKind != JsonValueKind.Object)
                {
                    _logger.LogError("Playwright scraping script returned invalid data.");
                    throw new Exception("Scraping script failed or returned null.");
                }

                // Safe helper: extract a list from rawData or return an empty list
                List<string> SafeList(string name) =>
                    rawData.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.Array
                        ? prop.EnumerateArray().Select(x => x.GetString() ?? "").ToList()
                        : new List<string>();

                // Safe helper: extract string from rawData or return empty
                string SafeString(string name) =>
                    rawData.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String
                        ? prop.GetString() ?? ""
                        : "";

                // Safe helper: extract int from rawData or return 0
                int SafeInt(string name) =>
                    rawData.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.Number
                        ? prop.GetInt32()
                        : 0;

                // Safe helper: extract double from rawData or return 0
                double SafeDouble(string name) =>
                    rawData.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.Number
                        ? prop.GetDouble()
                        : 0;

                // Safe helper: extract boolean from rawData or return false
                bool SafeBool(string name) =>
                    rawData.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.True
                        ? true
                        : false;

                
                // Grabbing html elements for pattern analysis
                var layoutElements = await _layoutParsingService.ExtractHtmlElementsAsync(page);
                
                // Use injected helper script to download the actual content of all external stylesheets
                var externalCssContents = await page.EvaluateAsync<string[]>("() => window.scrapeExternalCss()");

                // Use injected helper script to download the actual content of all external JS files
                var externalJsContents = await page.EvaluateAsync<string[]>("() => window.scrapeExternalJs()");


                // Close the page and browser context
                await page.CloseAsync();
                await context.CloseAsync();

                // Return structured scraping result as model
                return new ScrapedContent
                {
                    Url = url,
                    HtmlContent = SafeString("htmlContent"),
                    Headings = SafeInt("headings"),
                    Paragraphs = SafeInt("paragraphs"),
                    Images = SafeInt("images"),
                    Links = SafeList("links"),
                    TextContent = SafeString("text_content"),
                    Fonts = SafeList("fonts"),
                    HasFavicon = SafeBool("hasFavicon"),
                    FaviconUrl = SafeString("faviconUrl"),
                    ScrollHeight = SafeDouble("scrollHeight"),
                    ViewportHeight = SafeDouble("viewportHeight"),
                    ScrollWidth = SafeDouble("scrollWidth"),
                    ViewportWidth = SafeDouble("viewportWidth"),
                    ViewportLabel = SafeString("viewportLabel"),
                    InlineCssList = SafeList("inlineCssList"),
                    InlineJsList = SafeList("inlineJsList"),
                    ExternalCssLinks = SafeList("externalCssLinks"),
                    ExternalJsLinks = SafeList("externalJsLinks"),
                    ExternalCssContents = externalCssContents.ToList(),
                    ExternalJsContents = externalJsContents.ToList(),
                    InlineCss = string.Join("\n", SafeList("inlineCssList")),
                    InlineJs = string.Join("\n", SafeList("inlineJsList")),
                    LayoutElements = layoutElements
                };
            }
            catch (Exception ex)
            {
                // Catch any Playwright exception and log it
                _logger.LogError(ex, "Playwright scraping failed for URL: {Url}", url);

                // Rethrow to let the controller or caller handle it
                throw; 
            }
        }

        public async Task<ScrapedContent> ScrapeAsync(string url)
        {
            // Run in unique headless browser
            var browser = await _playwrightService.GetBrowserAsync(); 
            // var context = await browser.NewContextAsync();   
            // Ignore the page's Content Security Policy (CSP) to allow all resources to load
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                BypassCSP = true, // Bypass Content Security Policy
            });
            var page    = await context.NewPageAsync(); 

            // Navigate to the URL and wait until network is idle 
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

            // Inject custom JavaScript helpers into the page context
            await page.AddScriptTagAsync(new PageAddScriptTagOptions
            {
                Path = "wwwroot/js/playwrightScraperHelper.js"
            });

            // Extract all inline <style> tag content
            var inlineCss = await page.EvalOnSelectorAllAsync<string[]>("style", "elements => elements.map(e => e.textContent)");

            // Extract all external stylesheet links
            var externalCss = await page.EvalOnSelectorAllAsync<string[]>("link[rel='stylesheet']", "elements => elements.map(e => e.href)");

            // Extract all inline <script> tag content
            var inlineJs = await page.EvalOnSelectorAllAsync<string[]>("script:not([src])", "elements => elements.map(e => e.textContent)");

            // Extract all external JavaScript links
            var externalJs = await page.EvalOnSelectorAllAsync<string[]>("script[src]", "elements => elements.map(e => e.src)");

            // Use helper functions injected via scrape-assets.js to fetch content from external CSS files
            var externalCssContents = await page.EvaluateAsync<string[]>("() => window.scrapeExternalCss()");

            // Use helper functions injected via scrape-assets.js to fetch content from external JS files
            var externalJsContents = await page.EvaluateAsync<string[]>("() => window.scrapeExternalJs()");

            var scrollHeight = await page.EvaluateAsync<double>("() => document.documentElement.scrollHeight");
            var viewportHeight = await page.EvaluateAsync<double>("() => window.innerHeight");
            var scrollWidth = await page.EvaluateAsync<double>("() => document.documentElement.scrollWidth");
            var viewportWidth = await page.EvaluateAsync<double>("() => window.innerWidth");

            await page.CloseAsync();
            await context.CloseAsync();

            // Return all the scraped asset information as a ScrapedContent object
            return new ScrapedContent
            {
                Url = url,
                InlineCss = string.Join("\n", inlineCss),
                InlineJs = string.Join("\n", inlineJs),
                InlineCssList = inlineCss.ToList(),
                InlineJsList = inlineJs.ToList(),
                ExternalCssLinks = externalCss.ToList(),
                ExternalJsLinks = externalJs.ToList(),
                ExternalCssContents = externalCssContents.ToList(),
                ExternalJsContents = externalJsContents.ToList(),
                ScrollHeight = scrollHeight,
                ViewportHeight = viewportHeight,
                ScrollWidth = scrollWidth,
                ViewportWidth = viewportWidth,
                ViewportLabel = $"{viewportWidth}x{viewportHeight}"


            };
        }

        // Asynchronously scrapes the target URL at multiple viewport sizes and returns a dictionary of ScrapedContent per size
        public async Task<Dictionary<string, ScrapedContent>> ScrapeAcrossViewportsAsync(string url, List<(int width, int height)> viewports)
        {
            // Get a reusable headless browser instance from the injected Playwright service
            var browser = await _playwrightService.GetBrowserAsync();

            // This dictionary will store the scraped results for each viewport label 
            var result = new Dictionary<string, ScrapedContent>();

            // Iterate over each specified screen size (width x height)
            foreach (var (width, height) in viewports)
            {
                // Create a new isolated browser context with the current viewport size
                var context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = width, Height = height },
                    BypassCSP = true // Bypass Content Security Policy
                });

                var page = await context.NewPageAsync();
                await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                await page.AddScriptTagAsync(new PageAddScriptTagOptions
                {
                    Path = "wwwroot/js/playwrightScraperHelper.js"
                });

                var inlineCss = await page.EvalOnSelectorAllAsync<string[]>("style", "elements => elements.map(e => e.textContent)");
                var externalCss = await page.EvalOnSelectorAllAsync<string[]>("link[rel='stylesheet']", "elements => elements.map(e => e.href)");
                var inlineJs = await page.EvalOnSelectorAllAsync<string[]>("script:not([src])", "elements => elements.map(e => e.textContent)");
                var externalJs = await page.EvalOnSelectorAllAsync<string[]>("script[src]", "elements => elements.map(e => e.src)");

                var externalCssContents = await page.EvaluateAsync<string[]>("() => window.scrapeExternalCss()");
                var externalJsContents = await page.EvaluateAsync<string[]>("() => window.scrapeExternalJs()");

                var scrollHeight = await page.EvaluateAsync<double>("() => document.documentElement.scrollHeight");
                var viewportHeight = await page.EvaluateAsync<double>("() => window.innerHeight");
                var scrollWidth = await page.EvaluateAsync<double>("() => document.documentElement.scrollWidth");
                var viewportWidth = await page.EvaluateAsync<double>("() => window.innerWidth");

                var content = new ScrapedContent
                {
                    Url = url,
                    InlineCss = string.Join("\n", inlineCss),
                    InlineJs = string.Join("\n", inlineJs),
                    InlineCssList = inlineCss.ToList(),
                    InlineJsList = inlineJs.ToList(),
                    ExternalCssLinks = externalCss.ToList(),
                    ExternalJsLinks = externalJs.ToList(),
                    ExternalCssContents = externalCssContents.ToList(),
                    ExternalJsContents = externalJsContents.ToList(),
                    ScrollHeight = scrollHeight,
                    ViewportHeight = viewportHeight,
                    ScrollWidth = scrollWidth,
                    ViewportWidth = viewportWidth,
                    ViewportLabel = $"{width}x{height}"
                };

                result[content.ViewportLabel] = content;

                await context.CloseAsync();
            }

            return result;
        }
    }
}
