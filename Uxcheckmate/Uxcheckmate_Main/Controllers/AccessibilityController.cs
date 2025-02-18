using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Uxcheckmate_Main.Controllers
{
    [Route("accessibility")]
    public class AccessibilityController : Controller
    {
        private readonly WebScraperService _webScraperService;
        private readonly ILogger<AccessibilityController> _logger;

        public AccessibilityController(WebScraperService webScraperService, ILogger<AccessibilityController> logger)
        {
            _webScraperService = webScraperService;
            _logger = logger;
        }
        [HttpPost("checkAccessibility")]
        public async Task<IActionResult> CheckAccessibility([FromForm] string targetUrl)
        {
            // Doing another URL is empty check
            if (string.IsNullOrEmpty(targetUrl)){
                return BadRequest("Target URL is required.");
            }
            // Gathering the html information from scrapper
            string htmlContent;
            try{
                htmlContent = await _webScraperService.FetchHtmlAsync(targetUrl);
            }catch (Exception ex){
                return StatusCode(500, $"Error fetching HTML: {ex.Message}");
            }
            // Creating a temp file with the html information for the temp localhost server
            // Since pa11y needs to read from a server
            var tempHtmlFilePath = Path.GetTempFileName() + ".html";
            try{
                await System.IO.File.WriteAllTextAsync(tempHtmlFilePath, htmlContent);
            }
            catch (Exception ex){
                return StatusCode(500, $"Error writing temporary HTML file: {ex.Message}");
            }
            // Connecting path to run javascript and setting of pa11y for use
            var nodeScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "runPa11y.js");
            var startInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = $"\"{nodeScriptPath}\" \"{tempHtmlFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            // Running the process
            try{
                using var process = new Process { StartInfo = startInfo };
                process.Start();
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();
                await Task.WhenAll(outputTask, errorTask);
                process.WaitForExit();
                string output = outputTask.Result;
                string error = errorTask.Result;
                if (!string.IsNullOrWhiteSpace(error)){
                    _logger.LogError("Node script error: {Error}", error);
                    return BadRequest($"Node error: {error}");
                }
                // Deserialize the JSON output from pa11y
                try{
                    var pa11yResult = JsonSerializer.Deserialize<Pa11yResult>(output);
                    return Ok(pa11yResult);
                }catch (JsonException jsonEx){
                    _logger.LogError("Error deserializing pa11y JSON: {Error}", jsonEx.Message);
                    return StatusCode(500, "Error processing pa11y results");
                }
            }catch (Exception ex){
                return StatusCode(500, $"Error running Node script: {ex.Message}");
            }
            finally{
                // Deleting the temp Html file
                if (System.IO.File.Exists(tempHtmlFilePath)){
                    System.IO.File.Delete(tempHtmlFilePath);
                }
            }
        }
    }
}
