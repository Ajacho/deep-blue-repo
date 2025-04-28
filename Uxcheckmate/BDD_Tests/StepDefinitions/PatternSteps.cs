using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;

namespace BDD_Tests.StepDefinitions
{
    [Binding]
    public class PatternSteps
    {
        private readonly IWebDriver _driver;
        private readonly HtmlTestServer _htmlTestServer;
        public PatternSteps(IWebDriver driver, HtmlTestServer htmlTestServer)
        {
            _driver = driver;
            _htmlTestServer = htmlTestServer;
        }
        [Then("the user should see the F Pattern issue")]
        public void ThenTheUserShouldSeeTheFPatternIssue()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            var section = wait.Until(d =>
                d.FindElement(By.XPath("//button[contains(., 'F Pattern')]"))
            );
            Assert.That(section.Displayed, Is.True, "Expected 'F Pattern' issue to be visible.");
        }
        [Then("the user should not see the F Pattern issue")]
        public void ThenTheUserShouldNotSeeTheFPatternIssue()
        {
            Assert.Throws<NoSuchElementException>(() =>
            {
                _driver.FindElement(By.XPath("//button[contains(., 'F Pattern')]"));
            }, "F Pattern issue was unexpectedly found.");
        }
        [Then("the user should see the Z Pattern issue")]
        public void ThenTheUserShouldSeeTheZPatternIssue()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            var section = wait.Until(d =>
                d.FindElement(By.XPath("//button[contains(., 'Z Pattern')]"))
            );
            Assert.That(section.Displayed, Is.True, "Expected 'Z Pattern' issue to be visible.");
        }
        [Then("the user should not see the Z Pattern issue")]
        public void ThenTheUserShouldNotSeeTheZPatternIssue()
        {
            Assert.Throws<NoSuchElementException>(() =>
            {
                _driver.FindElement(By.XPath("//button[contains(., 'Z Pattern')]"));
            }, "Z Pattern issue was unexpectedly found.");
        }
        [Then("the user should see the symmetry issue")]
        public void ThenTheUserShouldSeeTheSymmetryIssue()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            var section = wait.Until(d =>
                d.FindElement(By.XPath("//button[contains(., 'Symmetry')]"))
            );
            Assert.That(section.Displayed, Is.True, "Expected 'Symmetry' issue to be visible.");
        }
        [Then("the user should not see the symmetry issue")]
        public void ThenTheUserShouldNotSeeTheSymmetryIssue()
        {
            Assert.Throws<NoSuchElementException>(() =>
            {
                _driver.FindElement(By.XPath("//button[contains(., 'Symmetry')]"));
            }, "Symmetry issue was unexpectedly found.");
        }
    }
}