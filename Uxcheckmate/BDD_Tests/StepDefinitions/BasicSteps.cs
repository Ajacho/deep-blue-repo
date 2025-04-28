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
    public class BasicSteps
    {
        private readonly IWebDriver driver;
        private HomeController _controller;
        private IActionResult _result;
        private readonly ScenarioContext _scenarioContext;
        private ScrapedContent _scrapedData;
        private readonly HtmlTestServer _htmlTestServer;
        public BasicSteps(IWebDriver webDriver, ScenarioContext scenarioContext, HtmlTestServer htmlTestServer)
        {
            driver = webDriver;
            _scenarioContext = scenarioContext;
            _htmlTestServer = htmlTestServer;
        }
        [Given(@"their local site url is ""(.*)""")]
        public void GivenTheirLocalSiteUrlIs(string localUrl)
        {
            var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\BDD_Tests\htmlFiles"));
            _htmlTestServer.StartServer(contentRoot);
            WaitUntilAvailable(localUrl).Wait();
        }
        [Given("the user navigates to the site")]
        public void GivenTheUserNavigatesToTheSite()
        {
            driver.Navigate().GoToUrl("http://localhost:5000/");
        }
        [Given(@"the user logs in")]
        public void GivenTheUserLogsIn()
        {
            PerformLogin("testuser@tests.com", "1P@ssword2");
        }
        [Given(@"the user logs in with ""(.*)"" and ""(.*)""")]
        public void GivenTheUserLogsInWith(string email, string password)
        {
            PerformLogin(email, password);
        }
        private void PerformLogin(string email, string password)
        {
            driver.FindElement(By.LinkText("Login")).Click();
            var emailInput = driver.FindElement(By.Id("Input_Email"));
            var passwordInput = driver.FindElement(By.Id("Input_Password"));
            emailInput.Clear();
            emailInput.SendKeys(string.IsNullOrEmpty(email) ? "testuser@tests.com" : email);
            passwordInput.Clear();
            passwordInput.SendKeys(string.IsNullOrEmpty(password) ? "1P@ssword2" : password);
            var loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            loginButton.Click();
        }
        [When(@"the user enters a URL to analyze(?: with ""(.*)"")?")]
        public void WhenTheUserEntersAUrlToAnalyze(string url = "https://example.com")
        {
            driver.FindElement(By.Id("urlInput")).SendKeys(url);
        }
        [When("the user starts the analysis")]
        public void WhenTheUserStartsTheAnalysis()
        {
            var submitButton = driver.FindElement(By.Id("analyzeBtn"));
            submitButton.Click();
        }
        [When("the report view has loaded")]
        public void WHenReportViewHadLoaded(){
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            bool navigated = wait.Until(d => d.Url.Contains("/Home/Report"));
            Assert.That(navigated, Is.True, "Did not navigate to Report View.");
        }
        [Then("the user should see the result view")]
        public void ThenTheUserShouldSeeTheResultView()
        {
            Assert.That(driver.Url, Does.Contain("/Home/Report"), "Did not navigate to Report View.");
            Assert.That(driver.PageSource.Contains("Report"),Is.True, "Expected content not found on result view.");
        }

        [When("the user enters {string} to analyze")]
        public void WhenTheUserEntersToAnalyze(string url)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var input = wait.Until(d => d.FindElement(By.Id("urlInput")));
            input.Clear();
            input.SendKeys(url);
        }

        [Given("the user has generated a report for \"(.*)\"")]
        public void GivenTheUserHasGeneratedAReport(string siteUrl)
        {
            driver.Navigate().GoToUrl("http://localhost:5000");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(180));
            var input = wait.Until(d => d.FindElement(By.Id("urlInput")));
            input.Clear();
            input.SendKeys(siteUrl);

            var button = driver.FindElement(By.Id("analyzeBtn"));
            button.Click();

            wait.Until(d => d.FindElement(By.Id("reportContainer"))); // Wait for report
        }
        private async Task WaitUntilAvailable(string url, int timeoutSeconds = 15)
        {
            using var client = new HttpClient();
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalSeconds < timeoutSeconds){
                try{
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode){
                        return;
                    }
                }catch{
                    // Server not up
                }
                await Task.Delay(500);
            }
            throw new Exception($"Timed out waiting for local HTML server at {url}");
        }
    }
}
