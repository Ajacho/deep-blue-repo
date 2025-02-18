using Microsoft.AspNetCore.Mvc;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Uxcheckmate_Main.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UxCheckmateDbContext _context;
    private readonly IOpenAiService _openAiService; 
    private readonly IPa11yService _pa11yService;

    public HomeController(ILogger<HomeController> logger, UxCheckmateDbContext dbContext, IOpenAiService openAiService, IPa11yService pa11yService)
    {
        _logger = logger;
        _context = dbContext;
        _openAiService = openAiService;
        _pa11yService = pa11yService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(ReportUrl model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if model.Url is null or empty
        if (string.IsNullOrEmpty(model.Url))
        {
            ModelState.AddModelError("Url", "URL cannot be empty.");
            return View(model);
        }

        try
        {
            // Call OpenAiService for UX analysis
            var uxReports = await _openAiService.AnalyzeAndSaveUxReports(model.Url);

            // Call Pa11yService for accessibility analysis
            var pa11yReports = await _pa11yService.AnalyzeAndSaveAccessibilityReport(model.Url);

            // Pass the reports as a tuple to the view
            ViewBag.AnalyzedUrl = model.Url;
            return View("Results", new Tuple<IEnumerable<Report>, IEnumerable<Pa11yIssue>>(uxReports, pa11yReports));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing URL: {Url}", model.Url);
            ModelState.AddModelError(string.Empty, "Failed to analyze the URL.");
            return View(model);
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

   /* [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }*/
}
