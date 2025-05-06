using Microsoft.AspNetCore.Mvc;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;

namespace Uxcheckmate_Main.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreenshotController : ControllerBase
    {
        private readonly IScreenshotService _screenshotService;

        public ScreenshotController(IScreenshotService screenshotService)
        {
            _screenshotService = screenshotService;
        }

        [HttpPost]
        public async Task<IActionResult> Capture([FromBody] ReportUrl model)
        {
            if (string.IsNullOrWhiteSpace(model.Url))
                return BadRequest("URL is required.");

            try
            {
                var screenshot = await _screenshotService.CaptureScreenshot(
                    new Microsoft.Playwright.PageScreenshotOptions(),
                    model.Url
                );

                if (string.IsNullOrEmpty(screenshot))
                    return StatusCode(500, "Failed to capture screenshot.");

                return Ok(screenshot); // returns base64 string
            }
            catch (Exception ex)
            {
                // Log to console or any logging service
                Console.Error.WriteLine("Screenshot error: " + ex.Message);
                Console.Error.WriteLine("Stack trace: " + ex.StackTrace);

                return StatusCode(500, new
                {
                    error = "Screenshot failed",
                    message = ex.Message,
                    stack = ex.StackTrace
                });
            }
        }

    }
}
