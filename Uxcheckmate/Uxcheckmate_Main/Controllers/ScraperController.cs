using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uxcheckmate_Main.Services;
using System.Collections.Generic;

namespace Uxcheckmate_Main.Controllers
{
    [ApiController]
    [Route("scraper")]
    public class ScraperController : ControllerBase
    {
        private readonly WebScraperService _scraperService;

        public ScraperController(WebScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        [HttpGet("extract")]
        public async Task<IActionResult> Extract([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { error = "URL parameter is required." });
            }

            try
            {
                var extractedData = await _scraperService.ScrapeAsync(url); // ✅ Ensure this is a Dictionary<string, object>

                return Ok(new
                {
                    Url = url,
                    Headings = extractedData.TryGetValue("headings", out var headings) ? headings : 0,
                    Images = extractedData.TryGetValue("images", out var images) ? images : 0,
                    Links = extractedData.TryGetValue("links", out var links) ? links : 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
