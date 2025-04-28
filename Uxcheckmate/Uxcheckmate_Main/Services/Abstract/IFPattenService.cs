using Uxcheckmate_Main.Models;
using Microsoft.Playwright;

public interface IFPatternService
{
    Task<string> AnalyzeFPatternAsync(double viewportWidth, double viewportHeight, List<HtmlElement> elements);
}