using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;
using SeleniumExtras.WaitHelpers;

namespace Uxcheckmate.BDD_Tests.StepDefinitions
{
    [Binding]
    public class PopUpsSteps
    {
        private readonly IWebDriver _driver;
        private readonly string _scannedUrl = "https://momkage-lexy.github.io/";
        private readonly PlaywrightService _playwrightService;
        private readonly PlaywrightScraperService _playwrightScraperService;
        private ScrapedContent _scrapedData;
        private Dictionary<string, object> _mergedData;
        private readonly WebScraperService _scraperService;
        private readonly ScenarioContext _scenarioContext;
        private readonly string _html;

        public PopUpsSteps(IWebDriver driver, ScenarioContext scenarioContext)
        {
            _driver = driver;
            _scenarioContext = scenarioContext;
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // layout parsing
            var layoutParsingLogger = loggerFactory.CreateLogger<LayoutParsingService>();
            var layoutParsingService = new LayoutParsingService(layoutParsingLogger);

            // Web Scraper Instance
            var webScraperLogger = loggerFactory.CreateLogger<WebScraperService>();
            _scraperService = new WebScraperService(new HttpClient(), webScraperLogger);

            // Playwright Instance
            var playwrightLogger = loggerFactory.CreateLogger<PlaywrightService>();
            _playwrightService = new PlaywrightService(playwrightLogger);

            // Playwright Scraper Instance
            var playwrightScraperLogger = loggerFactory.CreateLogger<PlaywrightScraperService>();
            _playwrightScraperService = new PlaywrightScraperService(_playwrightService, playwrightScraperLogger, layoutParsingService);

        }

        private Dictionary<string, object> MergeScrapedData(Dictionary<string, object> htmlData, ScrapedContent assets)
        {
            // Combine data from both scrapers
            var mergedData = new Dictionary<string, object>(htmlData)
            {
                ["externalCssContents"] = assets.ExternalCssContents,
                ["externalJsContents"] = assets.ExternalJsContents,
                ["inlineCss"] = assets.InlineCss,
                ["inlineJs"] = assets.InlineJs,
                ["externalCssLinks"] = assets.ExternalCssLinks,
                ["externalJsLinks"] = assets.ExternalJsLinks,
                ["scrollHeight"] = assets.ScrollHeight,
                ["viewportHeight"] = assets.ViewportHeight
            };

            return mergedData;
        }

        [When("the user clicks the Pop Ups section")]
        public void WhenTheUserClicksThePopUpsSection()
        {
            // Init JS Executer
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Find Pop Ups Element
            var element = _driver.FindElement(By.XPath("//button[contains(., 'Pop Ups')]"));

            // Scroll the element into view
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);

            // Wait until the element is clickable
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));

            // Using Js to click because I was getting not clickable errors
            js.ExecuteScript("arguments[0].click();", element);

        }

        [Then("the Pop Ups section should be visible")]
        public void ThenThePopUpsSectionShouldBeVisible()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

            // Find Pop Ups text area
            var section = wait.Until(d => d.FindElement(By.XPath("//button[contains(., 'Pop Ups')]")));

            // Assert is is displayed
            Assert.That(section.Displayed, Is.True);
        }

        /*
        [Then("the pop ups row reports all detected pop ups")]
        public async Task ThenThePopUpsRowReportsAllDetectedPopUps()
        {
        */
        /*==============================================================================
                                    Backend to UI Testing

        Check: What is expected based on service calls is what is displayed on the UI
        ==============================================================================*/
        /*
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Scrape scanned site
        var htmlData = await _scraperService.ScrapeAsync(_scannedUrl);
        var jsData = await _playwrightScraperService.ScrapeAsync(_scannedUrl);

        // Merge data from scrappers
        var mergedData = MergeScrapedData(htmlData, jsData);
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        // Call service method to check for pop ups
        var popupsService = new PopUpsService(loggerFactory.CreateLogger<PopUpsService>());
        string expectedReport = await popupsService.RunPopupAnalysisAsync(_scannedUrl, mergedData);

        // Verify UI
        var content = wait.Until(driver =>
        {
            // Attempt to find the element in the DOM
            var el = driver.FindElement(By.Id("pop-ups"));

            // Only return the element if it is displayed AND contains some text
            return el.Displayed && !string.IsNullOrWhiteSpace(el.Text) ? el : null;
        });

        string actualReport = content.Text;
            
            // Improved assertions with better error messages
            if (!string.IsNullOrWhiteSpace(expectedReport))
            {
                Assert.That(actualReport, Does.Contain("pop ups"),
                    $"Expected popup warning but found: {actualReport}");
            }
            else
            {
                Assert.That(actualReport, Does.Not.Contain("pop ups").And.Not.Contain("modal").And.Not.Contain("dialog"),
                    $"Unexpected popup warning found: {actualReport}");
            }
       }
       */
    }
}
