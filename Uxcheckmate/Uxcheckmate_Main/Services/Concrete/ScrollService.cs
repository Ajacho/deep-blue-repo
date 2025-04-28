using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public class ScrollService : IScrollService
    {
        private readonly ILogger<ScrollService> _logger;

        public ScrollService(ILogger<ScrollService> logger)
        {
            _logger = logger;
        }

        // Main entry point for the scroll analysis
        public async Task<string> RunScrollAnalysisAsync(string url, Dictionary<string, object> scrapedData)
        {
            // Ensure scrollHeight is available and valid
            if (!scrapedData.TryGetValue("scrollHeight", out var scrollHeightObj) || scrollHeightObj is not double scrollHeight)
            {
                _logger.LogWarning("Scroll height missing for scroll depth analysis at URL: {Url}", url);
                return string.Empty;
            }

            // Ensure viewportHeight is available and valid
            if (!scrapedData.TryGetValue("viewportHeight", out var viewportHeightObj) || viewportHeightObj is not double viewportHeight)
            {
                _logger.LogWarning("Viewport height missing for scroll depth analysis at URL: {Url}", url);
                return string.Empty;
            }

            // Call the internal analysis method
            return await AnalyzeScrollDepthAsync(scrollHeight, viewportHeight);
        }

        public async Task<string> AnalyzeScrollDepthAsync(double scrollHeight, double viewportHeight)
        {
            return await Task.Run(() =>
            {
                // Prevent division by zero or invalid dimensions
                if (viewportHeight <= 0)
                {
                    _logger.LogWarning("Invalid viewport height detected: {ViewportHeight}", viewportHeight);
                    return string.Empty;
                }

                // Calculate the number of full-screen scrolls needed to view the page
                int scrollCount = (int)Math.Ceiling(scrollHeight / viewportHeight);
                _logger.LogInformation("Scroll count estimated at: {ScrollCount}", scrollCount);

                // Return insight based on scroll length category
                if (scrollCount <= 3)
                {
                    return string.Empty;
                }
                else
                {
                    return $"This page requires about {scrollCount} scrolls to reach the bottom. You may want to streamline content or offer navigation aids.";
                }
            });
        }
    }
}
