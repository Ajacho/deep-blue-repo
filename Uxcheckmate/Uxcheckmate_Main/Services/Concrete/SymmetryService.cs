using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uxcheckmate_Main.Models;

public class SymmetryService : ISymmetryService
{
    private readonly ILogger<SymmetryService> _logger;
    public SymmetryService(ILogger<SymmetryService> logger)
    {
        _logger = logger;
    }

    public async Task<string> AnalyzeSymmetryAsync(double viewportWidth, double viewportHeight, List<HtmlElement> elements)
    {
        if (elements == null || elements.Count == 0){
            _logger.LogWarning("No layout elements found for symmetry analysis.");
            return "No layout elements found.";
        }
        double centerX = viewportWidth / 2;
        var leftElements = elements.Where(e => e.X + e.Width / 2 < centerX).ToList();
        var rightElements = elements.Where(e => e.X + e.Width / 2 >= centerX).ToList();
        double leftDensity = leftElements.Sum(e => e.Density);
        double rightDensity = rightElements.Sum(e => e.Density);
        double centerTolerance = 0.1 * viewportWidth;
        double centerDensity = elements.Where(e => Math.Abs((e.X + e.Width / 2) - centerX) <= centerTolerance).Sum(e => e.Density);
        _logger.LogDebug($"Left Density: {leftDensity:F4}, Right Density: {rightDensity:F4}");
        double totalDensity = leftDensity + rightDensity + centerDensity;
        if (totalDensity == 0){
            return "No visible elements to analyze.";
        }
        double symmetryScore = 1.0 - Math.Abs(leftDensity - rightDensity) / totalDensity;
        symmetryScore = Math.Round(symmetryScore, 2);
        string summary = $"Symmetry Score: {symmetryScore:P0}\n";
        if (symmetryScore >= 0.8){
            _logger.LogDebug($"Good symmetry detected (Score: {symmetryScore:P0})");
            return "";
        }else{
            _logger.LogDebug($"Low symmetry detected (Score: {symmetryScore:P0})");
            summary += "This layout does not have good left-right symmetry.";
            return await Task.FromResult(summary);
        }
    }
}
