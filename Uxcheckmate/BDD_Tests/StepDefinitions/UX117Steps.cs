using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reqnroll;
using Microsoft.Extensions.Logging;
using Uxcheckmate_Main.Services;

namespace BDD_Tests.StepDefinitions
{
    [Binding]
    public class UX117Steps
    {
        private readonly IWebDriver _driver;
        private readonly ScenarioContext _scenarioContext;

        public UX117Steps(IWebDriver driver, ScenarioContext scenarioContext){
            _driver = driver;
            _scenarioContext = scenarioContext;
        }
        [Then("the user will see a modal containing the summary")]
        public void ThenHeWillSeeAModalContainingSummary()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
            var summary = wait.Until(driver =>
            {
                try
                {
                    var modal = driver.FindElement(By.Id("onLoadModal"));
                    var summaryEl = modal.FindElement(By.Id("summary"));
                    return summaryEl.Displayed && !string.IsNullOrWhiteSpace(summaryEl.Text) ? summaryEl : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            Assert.That(summary.Displayed, Is.True, "Summary should be visible inside modal.");
        }
        [Then("the user clicks the let's begin button")]
        public void TheUserClicksTheLetsBeginButton()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(300));
            var button = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("letsgo")));
            // Find and click the let's begin button
          //  var button = wait.Until(driver => driver.FindElement(By.Id("letsgo")));
            button.Click();
        }
        [Then("the modal will close")]
        public void TheModalWillClose()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            bool isClosed = wait.Until(driver =>
            {
                try
                {
                    var modal = driver.FindElement(By.Id("onLoadModal"));
                    var style = modal.GetAttribute("style") ?? "";
                    var classAttr = modal.GetAttribute("class") ?? "";
                    Console.WriteLine($"Modal style: {style}");
                    Console.WriteLine($"Modal class: {classAttr}");
                    bool styleHidden = style.Contains("display: none");
                    bool classHidden = !classAttr.Contains("show");
                    return styleHidden || classHidden;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
            });
            Assert.That(isClosed, Is.True, "Modal should be closed or hidden.");
        }
    }
}