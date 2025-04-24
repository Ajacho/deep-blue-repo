using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Uxcheckmate_Main.Models;
public interface IPlaywrightScraperService
{
    Task<ScrapedContent> ScrapeAsync(string url);
    Task<Dictionary<string, ScrapedContent>> ScrapeAcrossViewportsAsync(string url, List<(int width, int height)> viewports);
    Task<ScrapedContent> ScrapeEverythingAsync(string url);
}