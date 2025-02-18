using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Uxcheckmate_Main.Services
{
    public class WebScraperService
    {
        private readonly HttpClient _httpClient;

        public WebScraperService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> FetchHtmlAsync(string url)
        {
            try
            {
                return await _httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching HTML content: {ex.Message}");
            }
        }

        public Dictionary<string, object> ExtractHtmlElements(string htmlContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var headings = doc.DocumentNode.SelectNodes("//h1 | //h2 | //h3 | //h4 | //h5 | //h6") ?? new HtmlNodeCollection(null);
            var paragraphs = doc.DocumentNode.SelectNodes("//p") ?? new HtmlNodeCollection(null);
            var images = doc.DocumentNode.SelectNodes("//img") ?? new HtmlNodeCollection(null);
            var links = doc.DocumentNode.SelectNodes("//a") ?? new HtmlNodeCollection(null);

            // Extract font-family styles from <style> and inline elements
            var fontStyles = doc.DocumentNode.SelectNodes("//style | //*[@style]") ?? new HtmlNodeCollection(null);
            var fontsUsed = new HashSet<string>();

            foreach (var node in fontStyles)
            {
                string style = node.InnerText + node.GetAttributeValue("style", "");
                var matches = System.Text.RegularExpressions.Regex.Matches(style, @"font-family:\s*([^;]+)");
                
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    fontsUsed.Add(match.Groups[1].Value.Trim());
                }
            }

            return new Dictionary<string, object>
            {
                { "headings", headings.Count },
                { "paragraphs", paragraphs.Count },
                { "images", images.Count },
                { "links", links.Count },
                { "text_content", string.Join("\n", paragraphs.Select(p => p.InnerText.Trim()).Where(text => !string.IsNullOrEmpty(text))) },
                { "fonts", fontsUsed.ToList() }
            };
        }

        public async Task<Dictionary<string, object>> ScrapeAsync(string url)
        {
            var htmlContent = await FetchHtmlAsync(url);
            return ExtractHtmlElements(htmlContent);
        }
    }
}