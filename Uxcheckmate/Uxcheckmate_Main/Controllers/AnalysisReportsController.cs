/*using Microsoft.AspNetCore.Mvc;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uxcheckmate_Main.Controllers
{
    [Route("analysis")]
    [ApiController]
    public class AnalysisReportsController : Controller
    {
        private readonly IOpenAiService _openAiService;
        private readonly IPa11yService _pa11yService;

        public AnalysisReportsController(IOpenAiService openAiService, IPa11yService pa11yService)
        {
            _openAiService = openAiService;
            _pa11yService = pa11yService;
        }

        // Change return type from JSON to ViewResult
        [HttpGet("all-reports/{url}")]
        public async Task<IActionResult> GetAnalysisReports(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL cannot be empty.");
            }

            // Call OpenAiApiService for UX analysis
            var uxReports = await _openAiService.AnalyzeAndSaveDesignIssues(url);

            // Call Pa11yService for accessibility analysis
            var pa11yReports = await _pa11yService.AnalyzeAndSaveAccessibilityReport(url);

            return Ok(new { Item1 = uxReports, Item2 = pa11yReports });
        }
    }
}*/