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
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("scanningWrapper")));
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
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var screenshotImage = wait.Until(d => d.FindElement(By.Id("screenshotImage")));
            string src = screenshotImage.GetAttribute("src");
            Assert.That(src, Is.Not.Null.And.Not.Empty, "Screenshot src is missing.");
            Assert.That(src, Does.StartWith("data:image/"), "Screenshot src is not a base64 image.");
        }


    }
}
