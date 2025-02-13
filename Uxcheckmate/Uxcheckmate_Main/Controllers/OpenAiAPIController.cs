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
        // Check if the URL parameter is missing or empty
        if (string.IsNullOrEmpty(url))
        {
            // Return a 400 Bad Request response if the URL is not provided
            return BadRequest("URL is required.");
        }

        // Call the OpenAI service to analyze the UX of the provided webpage
        string result = await _OpenAiService.AnalyzeUx(url);

        // Return the AI-generated UX recommendations as a JSON response
        return Content(result, "application/json");
    }
}