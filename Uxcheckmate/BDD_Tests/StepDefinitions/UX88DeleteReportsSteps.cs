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

namespace BDD_Tests.StepDefinitions
{
    [Binding]
    public class UX88DeleteReportsSteps
    {
        private readonly IWebDriver driver;
        private HomeController _controller;
        private IActionResult _result;
        public UX88DeleteReportsSteps(IWebDriver webDriver)
        {
            driver = webDriver;
        }

        /** 
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

        [When("they click user dash")]
        public async Task WhenTheyClickUserDash()
        {
            _result = await _controller.UserDash();
        }
        [Then("they should be in the user dash page")]
        public void ThenTheyShouldSeeALogout()
        {
            var viewResult = _result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.Null.Or.Empty);
        }

        */

        [When("the user clicks on one grouped folder of one domain")]
        public void WhenUserClicksOnGroupedFolder()
        {
            var groupedFolder = driver.FindElement(By.ClassName("container-report-list"));
            groupedFolder.Click();
        }

        [Then("they will see a delete button")]
        public void ThenTheyWillSeeDeleteButton()
        {
            var deleteButton = driver.FindElement(By.ClassName("deleteReportbtn"));
            Assert.That(deleteButton.Displayed, Is.True, "Delete button is not displayed.");
        }
    }
 }