using Uxcheckmate_Main.Models;
using Microsoft.Playwright;

public interface IZPatternService
{
    Task<string> AnalyzeZPatternAsync(double viewportWidth, double viewportHeight, List<HtmlElement> elements);
}