using Microsoft.AspNetCore.Mvc;
using Moq;
using Reqnroll;
using System.Security.Claims;
using Uxcheckmate_Main.Controllers;
using Microsoft.Extensions.Logging;
using Uxcheckmate_Main.Services;
using Uxcheckmate_Main.Models;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BDD_Tests.StepDefinitions
{
    [Binding]
    public class ScreenshotSteps
    {
        private readonly IWebDriver driver;
        public ScreenshotSteps(IWebDriver webDriver)
        {
            driver = webDriver;
        }
        [Then("the system displays a loading overlay with the website screenshot")]
        public void ThenTheSystemDisplaysLoadingOverlayWithScreenshot()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(driver => driver.FindElement(By.Id("scanningWrapper")));
            var screenshotElement = driver.FindElement(By.Id("screenshotPreview"));
            Assert.That(screenshotElement.Displayed, Is.True, "Screenshot element is not displayed.");
        }

        [Then("the report view is displayed")]
        public void ThenUserSeesLoadingOverlayWithScreenshot()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            var container = wait.Until(d => d.FindElement(By.Id("reportContainer")));
            Assert.That(container.Displayed, Is.True);
        }

        [Then("the user will see a screenshot of their website")]
        public void ThenUserSeesReportWithScreenshot()
        {
            Assert.That(driver.Url, Does.Contain("/Home/Report"), "Did not navigate to Report View.");
            Assert.That(driver.PageSource.Contains("Report"), Is.True, "Expected content not found on result view.");
            Assert.That(driver.PageSource.Contains("websiteScreenshot"), Is.True, "Expected screenshot not found on report view.");
        }

    }
}
