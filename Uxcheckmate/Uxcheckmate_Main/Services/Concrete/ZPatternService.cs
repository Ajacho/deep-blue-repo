using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public class ZPatternService : IZPatternService
    {
        private readonly ILogger<ZPatternService> _logger;
        public ZPatternService(ILogger<ZPatternService> logger)
        {
            _logger = logger;
        }
        public async Task<string> AnalyzeZPatternAsync(double viewportWidth, double viewportHeight, List<HtmlElement> elements)
        {
            if (elements == null || elements.Count == 0){
                _logger.LogWarning("No layout elements found for Z-pattern analysis.");
                return "No layout elements found.";
            }
            bool topLeftExists = elements.Any(e => e.X < 0.25 * viewportWidth && e.Y < 0.25 * viewportHeight);
            bool topRightExists = elements.Any(e => e.X > 0.75 * viewportWidth && e.Y < 0.25 * viewportHeight);
            bool bottomRightExists = elements.Any(e => e.X > 0.75 * viewportWidth && e.Y > 0.75 * viewportHeight);
            bool diagonalFlowExists = elements.Any(e => Math.Abs((e.Y / viewportHeight) - (e.X / viewportWidth)) <= 0.25);
            _logger.LogDebug($"Top Left: {topLeftExists}, Top Right: {topRightExists}, Diagonal: {diagonalFlowExists}, Bottom Right: {bottomRightExists}");
            int score = 0;
            if (topLeftExists){ 
                score += 25;
            }
            if (topRightExists){
                score += 25;
            }
            if (diagonalFlowExists){
                score += 25;
            }
            if (bottomRightExists){ 
                score += 25;
            }
            double finalScore = score / 100.0;
            string summary = $"Average Z-Pattern Score: {finalScore:P0}\n";
            if (finalScore >= 0.7){
                _logger.LogDebug($"Final Z-Pattern Score: {finalScore:P0}");
                return "";
            }else{
                _logger.LogDebug($"Final Z-Pattern Score: {finalScore:P0}");
                summary += "This layout does not follow the Z-pattern well.";
                return await Task.FromResult(summary);
            }
        }
    }
}
