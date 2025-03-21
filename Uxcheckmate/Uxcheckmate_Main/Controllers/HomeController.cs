using Microsoft.AspNetCore.Mvc;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Diagnostics;

namespace Uxcheckmate_Main.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient; 
    private readonly UxCheckmateDbContext _context;
    private readonly IOpenAiService _openAiService; 
    private readonly IReportService _reportService;
    private readonly IAxeCoreService _axeCoreService;
    private readonly PdfExportService _pdfExportService;

    public HomeController(ILogger<HomeController> logger, HttpClient httpClient, UxCheckmateDbContext dbContext, 
        IOpenAiService openAiService, IAxeCoreService axeCoreService, IReportService reportService, PdfExportService pdfExportService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _context = dbContext;
        _axeCoreService = axeCoreService;
        _reportService = reportService;
        _pdfExportService = pdfExportService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Report(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            ModelState.AddModelError("url", "URL cannot be empty.");
            return View("Index");
        }

       try
        {
            // Check if the URL is reachable
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Head, url);
                // Sending a HEAD request to the URL to check if it is reachable
                HttpResponseMessage response;
                try{
                    response = await httpClient.SendAsync(request);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "The URL is unreachable: {Url}", url);
                    TempData["UrlUnreachable"] = "The URL you entered seems incorrect or no longer exists. Please try again.";
                    return RedirectToAction("Index");
                }
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("The URL is unreachable: {Url}", url);
                    TempData["UrlUnreachable"] = "The URL you entered seems incorrect or no longer exists. Please try again.";
                    return RedirectToAction("Index");
                }
            }
            // Create and save the report record.
            var report = new Report
            {
                Url = url,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                AccessibilityIssues = new List<AccessibilityIssue>(),
                DesignIssues = new List<DesignIssue>()
            };
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Report record created with ID: {ReportId}", report.Id);
            
            await _axeCoreService.AnalyzeAndSaveAccessibilityReport(report);
            await _reportService.GenerateReportAsync(report);

            // Fetch the full report 
            var fullReport = await _context.Reports
                .Include(r => r.AccessibilityIssues)
                .Include(r => r.DesignIssues) // Load design issues
                .FirstOrDefaultAsync(r => r.Id == report.Id);

            if (fullReport == null)
            {
                _logger.LogError("Failed to fetch report with ID: {ReportId}", report.Id);
                ModelState.AddModelError("", "An error occurred while fetching the report.");
                return View("Index");
            }

            return View("Results", fullReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report for URL: {Url}", url);
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            return View("Index");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Feedback()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ErrorPage()
    {
        return View("ErrorPage");
    }

    [HttpGet]
    public IActionResult Disclosure()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TempBadPageExHeadersAndColors()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Guide()
    {
        // Grabing only ones with ScanMethod Custom for now
        var designCategories = await _context.DesignCategories.Where(dc => dc.ScanMethod == "Custom").ToListAsync();
        return View(designCategories);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadReport(int id)
    {
        var report = await _context.Reports
            .Include(r => r.AccessibilityIssues)
            .Include(r => r.DesignIssues)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
        {
            return NotFound("Report not found.");
        }

        var pdfBytes = _pdfExportService.GenerateReportPdf(report);
        return File(pdfBytes, "application/pdf", $"UXCheckmate_Report_{report.Id}.pdf");
    }
}