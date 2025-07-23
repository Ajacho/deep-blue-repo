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
    public class RoleBasedAccessSteps
    {
        private readonly IWebDriver driver;
        private HomeController _controller;
        private IActionResult _result;

        public RoleBasedAccessSteps(IWebDriver webDriver)
        {
            driver = webDriver;
        }

        /**
        [Given("user is logged in as an \"(.*)\"")]
        public void GivenUserIsLoggedInAsRole(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, "testUserId"),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            var httpContext = new DefaultHttpContext { User = user };
            var options = new DbContextOptionsBuilder<UxCheckmateDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var context = new UxCheckmateDbContext(options);

            context.Reports.Add(new Report
            {
                Id = 1,
                Url = "https://example.com",
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                UserID = "testUserId",
                AccessibilityIssues = new List<AccessibilityIssue>(),
                DesignIssues = new List<DesignIssue>()
            });
            context.SaveChanges();

            _controller = TestBuilder.BuildHomeController(httpContext, context);
        }

        [When("they visit the dashboard")]
        public async Task WhenTheyVisitTheDashboard()
        {
            _result = await _controller.UserDash();
        }

        [Then("they should see the \"(.*)\" view")]
        public void ThenTheyShouldSeeTheView(string expectedView)
        {
            var viewResult = _result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName ?? "UserDashboard", Is.EqualTo(expectedView).IgnoreCase);
        }

        [Then("they should land on the correct page in browser")]
        public void ThenTheyShouldLandOnCorrectPageInBrowser()
        {
            var dashboardLink = driver.FindElement(By.LinkText("Dashboard"));
            dashboardLink.Click();
            string currentUrl = driver.Url;
            Assert.That(currentUrl, Does.Contain("/Admin"), "Admin did not land on the Admin Dashboard.");
        }
        */
    }
}
