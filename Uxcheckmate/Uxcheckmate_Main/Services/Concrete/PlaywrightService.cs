using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Uxcheckmate_Main.Services
{
    // PlaywrightService is the centralized service for all Playwright operations. 
    // Avoiding redundant initialization.
    public class PlaywrightService : IPlaywrightService, IDisposable
    {
        private readonly ILogger<PlaywrightService> _logger;
        private IPlaywright _playwright;
        private IBrowser _browser;

        public PlaywrightService(ILogger<PlaywrightService> logger)
        {
            _logger = logger;
        }

        // This method initializes the browser instance if it doesn't exist.
        public async Task<IBrowser> GetBrowserAsync()
        {
            // Check if Playwright instance is null, if so, create a new instance
            if (_playwright == null)
            {
                _playwright = await Playwright.CreateAsync();
                _logger.LogInformation("Playwright instance created.");
            }
            // Check if browser instance is null, if so, create a new instance
            if (_browser == null)
            {
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                // Use the path to the Chromium executable in Azure App Service
                ExecutablePath = "/home/site/wwwroot/.playwright/chromium/chrome-linux/chrome",
                Args = new[]
                {
                    "--disable-gpu",
                    "--no-sandbox",
                    "--disable-dev-shm-usage"
                }
            });

                _logger.LogInformation("Playwright browser instance created.");
            }

            return _browser; // Return the browser instance
        }

        // This method creates a fresh browser context per request (avoiding cross-session state)
        public async Task<IBrowserContext> GetBrowserContextAsync()
        {
            var browser = await GetBrowserAsync(); // Get the browser instance
            // return await browser.NewContextAsync(); // Create and return a new browser context
            return await browser.NewContextAsync(new BrowserNewContextOptions
            {
                // Ignore the page's Content Security Policy (CSP) to allow all resources to load
                BypassCSP = true, // Bypass Content Security Policy
            });
        }

        // Gracefully closes the browser and Playwright instance.
        public async Task CloseBrowserAsync()
        {
            // Check if browser instance is not null, if so, close the browser instance
            if (_browser != null)
            {
                await _browser.CloseAsync();
                _browser = null;
                _logger.LogInformation("Playwright browser instance closed.");
            }
            // Check if Playwright instance is not null, if so, dispose the Playwright instance
            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
                _logger.LogInformation("Playwright instance disposed.");
            }
        }

        // Automatically clean up resources if service is disposed
        public void Dispose()
        {
            // Check if Playwright instance is not null, if so, dispose the Playwright instance
            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
                _logger.LogInformation("Playwright disposed on service shutdown.");
            }
        }
    }
}
