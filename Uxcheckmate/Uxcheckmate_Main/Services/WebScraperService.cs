using System;
using System.Collections.Generic;
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

            var headings = doc.DocumentNode.SelectNodes("//h2") ?? new HtmlNodeCollection(null);
            var images = doc.DocumentNode.SelectNodes("//img") ?? new HtmlNodeCollection(null);
            var links = doc.DocumentNode.SelectNodes("//a") ?? new HtmlNodeCollection(null);

            return new Dictionary<string, object>
            {
                { "headings", headings.Count },
                { "images", images.Count },
                { "links", links.Count }
            };
        }

        public async Task<Dictionary<string, object>> ScrapeAsync(string url)
        {
            var htmlContent = await FetchHtmlAsync(url);
            return ExtractHtmlElements(htmlContent);
        }
    }
}
