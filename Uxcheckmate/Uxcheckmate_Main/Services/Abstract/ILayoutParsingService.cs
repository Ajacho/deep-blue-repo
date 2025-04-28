using Uxcheckmate_Main.Models;
using Microsoft.Playwright;
public interface ILayoutParsingService
{
    Task<List<HtmlElement>> ExtractHtmlElementsAsync(IPage page);
}