using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uxcheckmate_Main.Models;
using Microsoft.Playwright;

namespace Uxcheckmate_Main.Services
{
    public class LayoutParsingService : ILayoutParsingService
    {
        private readonly ILogger<LayoutParsingService> _logger;

        public LayoutParsingService(ILogger<LayoutParsingService> logger)
        {
            _logger = logger;
        }
        public async Task<List<HtmlElement>> ExtractHtmlElementsAsync(IPage page)
        {
            try{
                var jsResult = await page.EvaluateAsync<JsonElement>(@"
                    () => {
                        return Array.from(document.querySelectorAll('h1, h2, h3, p, img, nav, a, button')).map(el => {
                            const rect = el.getBoundingClientRect();
                            return {
                                tag: el.tagName,
                                x: rect.left,
                                y: rect.top,
                                width: rect.width,
                                height: rect.height,
                                text: el.innerText
                            };
                        });
                    }
                ");
                var elements = new List<HtmlElement>();
                foreach (var element in jsResult.EnumerateArray()){
                    elements.Add(new HtmlElement
                    {
                        Tag = element.GetProperty("tag").GetString() ?? "",
                        X = element.GetProperty("x").GetDouble(),
                        Y = element.GetProperty("y").GetDouble(),
                        Width = element.GetProperty("width").GetDouble(),
                        Height = element.GetProperty("height").GetDouble(),
                        Text = element.GetProperty("text").GetString()
                    });
                }
                return elements;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error extracting element layout data.");
                return new List<HtmlElement>();
            }
        }
    }
}
