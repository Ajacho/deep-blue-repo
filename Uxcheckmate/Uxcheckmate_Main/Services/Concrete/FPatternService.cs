using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public class FPatternService : IFPatternService
    {
        private readonly ILogger<FPatternService> _logger;
        public FPatternService(ILogger<FPatternService> logger)
        {
            _logger = logger;
        }
        public async Task<string> AnalyzeFPatternAsync(double viewportWidth, double viewportHeight, List<HtmlElement> elements)
        {
            if (elements == null || elements.Count == 0){
                _logger.LogWarning("No layout elements provided found for F anaylsis.");
                return "No layout elements were found to analyze.";
            }
            // Create bands to check for multiple F patterns
            double fixedBandHeight = Math.Min(viewportHeight * 0.5, 800);
            double gapThreshold = 300;
            double leftZoneThreshold = 0.26 * viewportWidth;
            var sorted = elements.OrderBy(e => e.Y).ToList();
            List<List<HtmlElement>> bands = new();
            List<HtmlElement> currentBand = new() { sorted[0] };
            for (int i = 1; i < sorted.Count; i++){
                double gap = sorted[i].Y - (sorted[i - 1].Y + sorted[i - 1].Height);
                if (gap > gapThreshold || sorted[i].Y > currentBand[0].Y + fixedBandHeight){
                    bands.Add(currentBand);
                    currentBand = new();
                }
                currentBand.Add(sorted[i]);
            }
            if (currentBand.Count > 0){
                bands.Add(currentBand);
            }
            // Score Sections
            double globalAvgDensity = elements.Average(e => e.Density);
            double adaptiveThreshold = Math.Max(0.015, globalAvgDensity * 2.0);
            _logger.LogDebug($"Global Average Density: {globalAvgDensity:F4}, Adaptive Threshold: {adaptiveThreshold:F4}");
            List<double> sectionScores = new();
            List<double> weights = new();
            int includedBands = 0;
            for (int i = 0; i < bands.Count; i++){
                var band = bands[i];
                double totalDensity = band.Sum(e => e.Density);
                bool lowDensity = totalDensity < adaptiveThreshold;
                if (lowDensity){
                    _logger.LogDebug($"Low density band {i}: {totalDensity:F2} < {adaptiveThreshold:F2} — still evaluating but weighing less.");
                }
                double topBoundary = band.Min(b => b.Y) + 0.2 * fixedBandHeight;
                double middleBoundary = band.Min(b => b.Y) + 0.4 * fixedBandHeight;
                double topZoneDensity = band.Where(e => e.Y < topBoundary).Sum(e => e.Density);
                double middleZoneDensity = band.Where(e => e.Y >= topBoundary && e.Y < middleBoundary).Sum(e => e.Density);
                double leftZoneDensity = band.Where(e => e.X < leftZoneThreshold).Sum(e => e.Density);
                double score = (0.15 * topZoneDensity + 0.15 * middleZoneDensity + 0.7 * leftZoneDensity) / totalDensity;
                score = Math.Round(score, 2);
                _logger.LogDebug($"Band {i} Score => Top: {topZoneDensity:F2}, Middle: {middleZoneDensity:F2}, Left: {leftZoneDensity:F2}, Total: {totalDensity:F2}, Score: {score:F2}");
                sectionScores.Add(score);
                double weight = lowDensity ? 0.5 * Math.Max(0.3, 1.0 / (i + 1)) : Math.Max(0.3, 1.0 / (i + 1));
                weights.Add(weight);
                includedBands++;
            }
            if (sectionScores.Count == 0){
                return "No valid sections found for F-pattern analysis.";
            }
            // Averaging the scores
            double weightedAverage = sectionScores.Zip(weights, (score, w) => score * w).Sum() / weights.Sum();
            double generousAverage = Math.Round(Math.Min(1.0, weightedAverage + 0.20), 2); // Giving grace since not every part needs F pattern
            string summary = $"Average F-Pattern Score: {generousAverage:P0}\n";
            if (generousAverage >= 0.7){
                summary = ""; // Making blank
            }
            else{
                summary += "This layout does not follow the F-pattern well.";
            }
            return await Task.FromResult(summary);
        }
        private bool IsMostlyNonText(List<HtmlElement> band){
            int textElements = band.Count(e => !string.IsNullOrWhiteSpace(e.Text));
            return textElements <= 1;
        }
    }
}