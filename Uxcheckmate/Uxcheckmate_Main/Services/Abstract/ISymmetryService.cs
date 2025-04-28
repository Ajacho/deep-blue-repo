using Uxcheckmate_Main.Models;
using Microsoft.Playwright;

public interface ISymmetryService
{
    Task<string> AnalyzeSymmetryAsync(double viewportWidth, double viewportHeight, List<HtmlElement> elements);
}