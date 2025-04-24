using System.IO;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Moq;
using Moq_Tests;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json;
using Uxcheckmate_Main.Controllers;
using Uxcheckmate_Main.Services;
using Uxcheckmate_Main.Models;

namespace Database_Tests
{
    public class Database_Tests
    {
        private UxCheckmateDbContext _context;
        private DbContextOptions<UxCheckmateDbContext> _options;
    [SetUp]
    public void Setup()
    {
        var connectionJson = Environment.GetEnvironmentVariable("DB_STRING_SECRET");

        string dbConnectionString = null;
        string authDbConnectionString = null;

        if (!string.IsNullOrEmpty(connectionJson))
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(connectionJson);
            dbConnectionString = dict.GetValueOrDefault("DBConnection");
            authDbConnectionString = dict.GetValueOrDefault("AuthDBConnection");
        }
        else
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.WorkDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            dbConnectionString = configuration.GetConnectionString("DBConnection");
            authDbConnectionString = configuration.GetConnectionString("AuthDBConnection");
        }

        // Console.WriteLine($"DB: {dbConnectionString}");
        // Console.WriteLine($"AuthDB: {authDbConnectionString}");

        if (string.IsNullOrEmpty(dbConnectionString))
            throw new InvalidOperationException("DBConnection was not found!");

        _options = new DbContextOptionsBuilder<UxCheckmateDbContext>()
            .UseSqlServer(dbConnectionString)
            .Options;
        _context = new UxCheckmateDbContext(_options);
    }


        [Test]
        public void Database_Connected()
        {
            bool canConnect = _context.Database.CanConnect();
            Assert.That(canConnect, Is.True);
        }

        [Test]
        public void Read_Categories()
        {
            var DesignCategory = _context.DesignCategories.ToList();
            Assert.That(DesignCategory, Is.Not.Empty);
            Assert.That(DesignCategory.Any(d => d.Id == 18 && d.Name == "Visual Hierarchy"), Is.True);
            Assert.That(DesignCategory.Any(d => d.Id == 19 && d.Name == "Broken Links"), Is.True);
        }

        [Test]
        public void Add_Test_Issue_Test()
        {
            var report = new Report
            {
                Url = "https://fakenonexistenturl.com",
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                UserID = "testUser"
            };
            _context.Reports.Add(report);
            _context.SaveChanges();
            var category = new DesignIssue { CategoryId = 18, ReportId = report.Id, Message = "Test Message", Severity = 1 };
            _context.DesignIssues.Add(category);
            _context.SaveChanges();
            var testCategory = _context.DesignIssues.FirstOrDefault(c => c.Message == "Test Message");
            Assert.That(testCategory, Is.Not.Null);
            Assert.That(testCategory.CategoryId, Is.EqualTo(18));
            Assert.That(testCategory.ReportId, Is.EqualTo(report.Id));
            Assert.That(testCategory.Severity, Is.EqualTo(1));
        }

        // Singular URL Tests
        [Test]
        public async Task DeleteOldReportAndMakeNewOne()
        {
            var userId = "testUserID";
            var url = "https://example.com/";
            _context.Reports.Add(new Report
            {
                Url = url,
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                UserID = userId
            });
            await _context.SaveChangesAsync();
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }, "TestAuth"))
            };
            var controller = TestBuilder.BuildHomeController(httpContext, _context);
            await controller.Report(url);
            var userReports = await _context.Reports.Where(r => r.Url == url && r.UserID == userId).ToListAsync();
            Assert.That(userReports.Count, Is.EqualTo(1), "Only one report should remain after replacement.");
        }
        [Test]
        public async Task OtherUsersReportNotDeleted()
        {
            var url = "https://example.com/";
            var user1 = "testUserID1";
            var user2 = "testUserID2";
            _context.Reports.AddRange(
                new Report { Url = url, UserID = user1, Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)) },
                new Report { Url = url, UserID = user2, Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)) }
            );
            await _context.SaveChangesAsync();
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user1)
                }, "TestAuth"))
            };
            var controller = TestBuilder.BuildHomeController(httpContext, _context);
            await controller.Report(url);
            var reports = await _context.Reports.Where(r => r.Url == url).ToListAsync();
            Assert.That(reports.Count, Is.EqualTo(2), "Each user should have one report.");
            Assert.That(reports.Any(r => r.UserID == user1), Is.True);
            Assert.That(reports.Any(r => r.UserID == user2), Is.True);
        }
        [TearDown]
        public void TearDown()
        {
            // Cleaning the url test data
            var testUserIds = new[] { "testUserID", "testUserID1", "testUserID2" };
            var testReports = _context.Reports.Where(r => testUserIds.Contains(r.UserID));
            _context.Reports.RemoveRange(testReports);
            var testCategories = _context.DesignIssues.Where(c => c.Message == "Test Message");
            _context.DesignIssues.RemoveRange(testCategories);
            _context.SaveChanges();
            _context.Dispose();
        }
    }
}