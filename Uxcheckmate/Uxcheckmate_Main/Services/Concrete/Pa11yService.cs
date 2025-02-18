using System.Text.Json;
using System.Collections.Generic;
using Uxcheckmate_Main.Models;
using Microsoft.Extensions.Logging;

namespace Uxcheckmate_Main.Services
{
    public class Pa11yService : IPa11yService
    {
        private readonly Pa11yUrlBasedService _pa11yUrlBasedService;
        private readonly ILogger<Pa11yService> _logger;

        public Pa11yService(Pa11yUrlBasedService pa11yUrlBasedService, ILogger<Pa11yService> logger)
        {
            _pa11yUrlBasedService = pa11yUrlBasedService;
            _logger = logger;
        }

        public async Task<List<Pa11yIssue>> AnalyzeAndSaveAccessibilityReport(string url)
        {
            // Run Pa11y using the existing service and get the result as a JSON string
            var pa11yJsonResult = await Task.Run(() => _pa11yUrlBasedService.RunPa11y(url));

            // Log the JSON result
            _logger.LogInformation("Pa11y JSON Result: {Pa11yJsonResult}", pa11yJsonResult);

            // Deserialize the JSON result into Pa11yIssue list
            var pa11yIssues = DeserializePa11yResult(pa11yJsonResult);

            // Log the deserialized issues
            _logger.LogInformation("Deserialized Pa11y Issues: {Pa11yIssues}", pa11yIssues);

            // Return simplified issues
            return pa11yIssues ?? new List<Pa11yIssue>();
        }

        private List<Pa11yIssue> DeserializePa11yResult(string pa11yJsonResult)
        {
            try
            {
                // Deserialize the JSON result into a List of Pa11yIssue
                var issues = JsonSerializer.Deserialize<List<Pa11yIssue>>(pa11yJsonResult);

                return issues;
            }
            catch (JsonException ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error deserializing Pa11y JSON result");

                // Handle any JSON deserialization errors
                return new List<Pa11yIssue>();
            }
        }
    }
}
