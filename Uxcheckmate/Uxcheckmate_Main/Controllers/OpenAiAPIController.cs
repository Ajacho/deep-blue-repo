using Microsoft.AspNetCore.Mvc;
using Uxcheckmate_Main.Services;
using System.Threading.Tasks;

[ApiController]
[Route("api/chat")]
public class OpenAiApiController : Controller
{
    private readonly IOpenAiService _OpenAiService;
    private readonly ILogger<OpenAiApiController> _logger;

    public OpenAiApiController(IOpenAiService OpenAIService, ILogger<OpenAiApiController> logger)
    {
        _logger = logger;
        _OpenAiService = OpenAIService;
    }

    [HttpGet("analyze")]
    public async Task<IActionResult> AnalyzeUx([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest(new { error = "URL is required." });
        }

        var result = await _OpenAiService.AnalyzeAndSaveUxReports(url);
        return Ok(result);
    }
}
