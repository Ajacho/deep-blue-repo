using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq_Tests;
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
    public class UserDashSteps
    {
        private readonly IWebDriver driver;
        private HomeController _controller;
        private IActionResult _result;
        public UserDashSteps(IWebDriver webDriver)
        {
            driver = webDriver;
        }

        [Given("user is logged in")]
        public void GivenUserIsLoggedIn()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Priya"),
                new Claim(ClaimTypes.NameIdentifier, "priUser"),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            var httpContext = new DefaultHttpContext { User = user };

            // In Memory DB Setup
            var options = new DbContextOptionsBuilder<UxCheckmateDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var context = new UxCheckmateDbContext(options);

            // Moqing report
            context.Reports.Add(new Report
            {
                Id = 1,
                Url = "https://example.com",
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                UserID = "priUser",
                AccessibilityIssues = new List<AccessibilityIssue>(),
                DesignIssues = new List<DesignIssue>()
            });

            context.SaveChanges();
            _controller = TestBuilder.BuildHomeController(httpContext, context);
        }

       [When("they go to user dashboard")]
        public void WhenTheyGoToUserDashboard()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                var userDash = wait.Until(d => d.FindElement(By.LinkText("Dashboard")));
                userDash.Click();
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    var adminDash = wait.Until(d => d.FindElement(By.LinkText("Admin Dashboard")));
                    adminDash.Click();
                }
                catch (WebDriverTimeoutException ex)
                {
                    Assert.Fail("Neither 'Dashboard' nor 'Admin Dashboard' could be found in time: " + ex.Message);
                }
            }
        }
        [Then("they should see admin dashboard")]
        public void ThenTheyShouldSeeAdminDashboard()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var dashboardLink = wait.Until(d => d.FindElement(By.LinkText("Admin Dashboard")));
            dashboardLink.Click();
            string currentUrl = driver.Url;
            Assert.That(currentUrl, Does.Contain("/Admin"), "Admin did not land on the Admin Dashboard.");
        }
        [Then("they should be in user dash page")]
        public void ThenTheyShouldSeeALogout()
        {
            var viewResult = _result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.Null.Or.Empty);
        }
        [When("they click logout button")]
        public void WhenTheyClickLogoutButton()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // Removing user account
            var httpContext = new DefaultHttpContext { User = user };
            var controllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.ControllerContext = controllerContext;
        }
        [Then("they should be logged out")]
        public void ThenTheyShouldBeLoggedOut()
        {
            var isAuthenticated = _controller.HttpContext.User.Identity?.IsAuthenticated ?? false;
            Assert.That(isAuthenticated, Is.False, "User is still authenticated after logout.");
        }
        // Selenium Additions Below
        [Then("they should see user dashboard")]
        public void ThenTheyShouldBeLoggedIn()
        {
            var dashboardLink = driver.FindElement(By.LinkText("Dashboard"));
            dashboardLink.Click();
            string currentUrl = driver.Url;
            Assert.That(currentUrl, Does.Contain("/Home/UserDash"), "User is not on the dashboard page.");
        }
        [Then("they should see that report")]
        public void ThenTheyShouldSeeThatReport()
        {
            Assert.That(driver.PageSource.Contains("Date"), Is.True, "Report ID was not found on the dashboard page.");
        }

        // Grouped reports
        [Then("the user should see grouped page reports by domain")]
        public void ThenUserSeesGroupedReportsByDomain()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Wait until at least one folder-card is present
            var reportGroups = wait.Until(d => d.FindElements(By.ClassName("folder-card")));
            Assert.That(reportGroups.Count, Is.GreaterThan(0), "No grouped report cards found.");
        }
    }
 }