using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Uxcheckmate_Main.Controllers;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;

namespace Controller_Tests
{
    [TestFixture]
    public class HomeControllerTests : IDisposable  // Implement IDisposable for resource cleanup
    {
        private Mock<ILogger<HomeController>> _loggerMock;
        private Mock<HttpClient> _httpClientMock;
        private UxCheckmateDbContext _dbContext; 
        private Mock<IOpenAiService> _openAiServiceMock;
        private Mock<IReportService> _reportServiceMock;
        private Mock<IAxeCoreService> _axeCoreServiceMock;
        private Mock<PdfExportService> _pdfExportServiceMock;
        private Mock<IScreenshotService> _screenshotServiceMock;
        private HomeController _controller; 
        
        private Mock<DbSet<Report>> _mockReportDbSet;
        private Mock<ICompositeViewEngine> _viewEngineMock;
        private Mock<ITempDataDictionary> _tempDataMock;

        [SetUp]  
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _httpClientMock = new Mock<HttpClient>();
            _openAiServiceMock = new Mock<IOpenAiService>();
            _reportServiceMock = new Mock<IReportService>();
            _axeCoreServiceMock = new Mock<IAxeCoreService>();
            _pdfExportServiceMock = new Mock<PdfExportService>();
            _screenshotServiceMock = new Mock<IScreenshotService>();
            _viewEngineMock = new Mock<ICompositeViewEngine>();
            _tempDataMock = new Mock<ITempDataDictionary>();

            // Mock the CaptureScreenshot method
            _screenshotServiceMock
                .Setup(s => s.CaptureScreenshot(It.IsAny<Microsoft.Playwright.PageScreenshotOptions>(), It.IsAny<string>()))
                .ReturnsAsync("mockScreenshotPath");
                
            // Configure in-memory database options
            var options = new DbContextOptionsBuilder<UxCheckmateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // Unique name per test
                .Options;
            
            // Create actual DbContext instance with in-memory provider
            _dbContext = new UxCheckmateDbContext(options);

            // Mock view rendering service for partial views
            var viewRenderServiceMock = new Mock<IViewRenderService>();
            viewRenderServiceMock
                .Setup(m => m.RenderViewToStringAsync(It.IsAny<Controller>(), "_DesignIssuesPartial", It.IsAny<IEnumerable<DesignIssue>>()))
                .ReturnsAsync("<div>Design Issues</div>");
            viewRenderServiceMock
                .Setup(m => m.RenderViewToStringAsync(It.IsAny<Controller>(), "_AccessibilityIssuesPartial", It.IsAny<IEnumerable<AccessibilityIssue>>()))
                .ReturnsAsync("<div>Accessibility Issues</div>");

            // Instantiate controller with mocked dependencies
            _controller = new HomeController(
                _loggerMock.Object,
                _httpClientMock.Object,
                _dbContext,
                _openAiServiceMock.Object,
                _axeCoreServiceMock.Object,
                _reportServiceMock.Object,
                _pdfExportServiceMock.Object,
                _screenshotServiceMock.Object,
                viewRenderServiceMock.Object
            );

            // Configure TempData for controller
            _controller.TempData = _tempDataMock.Object;

            // Set up HttpContext with mocked service provider
            var httpContext = new DefaultHttpContext();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICompositeViewEngine)))
                .Returns(_viewEngineMock.Object);
            httpContext.RequestServices = serviceProviderMock.Object;
            
            // Assign context to controller
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Seed test data into in-memory database
            SetupTestData();
        }

        // Helper method to create initial test data
        private void SetupTestData()
        {
            // Create sample categories
            var designCategories = new List<DesignCategory>
            {
                new DesignCategory { Id = 1, Name = "Layout" },
                new DesignCategory { Id = 2, Name = "Color" }
            };
            
            var accessibilityCategories = new List<AccessibilityCategory>
            {
                new AccessibilityCategory { Id = 1, Name = "WCAG 2.1" },
                new AccessibilityCategory { Id = 2, Name = "Keyboard" }
            };

            // Add categories to context
            _dbContext.DesignCategories.AddRange(designCategories);
            _dbContext.AccessibilityCategories.AddRange(accessibilityCategories);
            _dbContext.SaveChanges();

            // Create test report with sample issues
            var testReport = new Report
            {
                Id = 1,
                Url = "https://example.com",
                Date = DateOnly.FromDateTime(DateTime.Now),
                DesignIssues = new List<DesignIssue>
                {
                    new DesignIssue { Id = 1, Message = "Layout issue 1", Severity = 3, CategoryId = 1 },
                    new DesignIssue { Id = 2, Message = "Layout issue 2", Severity = 1, CategoryId = 1 },
                    new DesignIssue { Id = 3, Message = "Color issue", Severity = 2, CategoryId = 2 }
                },
                AccessibilityIssues = new List<AccessibilityIssue>
                {
                    new AccessibilityIssue { Id = 1, Message = "WCAG issue", Details = "Details for WCAG issue", 
                        Selector = "#element1", WCAG = "1.1.1", Severity = 3, CategoryId = 1 },
                    new AccessibilityIssue { Id = 2, Message = "Keyboard issue 1", Details = "Details for keyboard issue 1", 
                        Selector = "#element2", WCAG = "2.1.1", Severity = 1, CategoryId = 2 },
                    new AccessibilityIssue { Id = 3, Message = "Keyboard issue 2", Details = "Details for keyboard issue 2", 
                        Selector = "#element3", WCAG = "2.1.2", Severity = 2, CategoryId = 2 }
                }
            };

            // Add report to context
            _dbContext.Reports.Add(testReport);
            _dbContext.SaveChanges();
        }

        [TearDown]  
        public void TearDown()
        {
            _controller.Dispose();  
        }

        public void Dispose()  
        {
            _controller?.Dispose();  // Ensure controller disposal
            _dbContext?.Dispose();   // Cleanup database context
        }

        [Test]
        public async Task GetSortedIssues_WithCategorySort_ReturnsIssuesSortedByCategory()
        {
            // Arrange - Get test report from database
            var report = await _dbContext.Reports
                .Include(r => r.DesignIssues)
                .Include(r => r.AccessibilityIssues)
                .FirstAsync(r => r.Id == 1);

            Assert.That(report, Is.Not.Null, "Test report should exist");

            // Act - Call controller method with "category" sort order
            var result = await _controller.GetSortedIssues(1, "category") as JsonResult;

            // Assert - Verify response structure and sorting
            Assert.That(result, Is.Not.Null, "Should return JSON result");
            Assert.That(_controller.ViewBag.CurrentSort, Is.EqualTo("category"), 
                "ViewBag should store current sort order");
            
            // Verify JSON structure contains expected properties
            var resultValue = result.Value;
            Assert.That(resultValue.GetType().GetProperty("designHtml"), Is.Not.Null, 
                "Should contain design HTML");
            Assert.That(resultValue.GetType().GetProperty("accessibilityHtml"), Is.Not.Null, 
                "Should contain accessibility HTML");
        }

        [Test]
        public async Task GetSortedIssues_WithSeverityHighLowSort_SetsSortOrderCorrectly()
        {
            // Act - Test high-to-low severity sorting
            var result = await _controller.GetSortedIssues(1, "severity-high-low") as JsonResult;

            // Assert - Verify sort order tracking
            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ViewBag.CurrentSort, Is.EqualTo("severity-high-low"), 
                "Should track current sort order");
        }

        [Test]
        public async Task GetSortedIssues_WithSeverityLowHighSort_SetsSortOrderCorrectly()
        {
            // Act - Test low-to-high severity sorting
            var result = await _controller.GetSortedIssues(1, "severity-low-high");

            // Assert - Verify sort order tracking
            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ViewBag.CurrentSort, Is.EqualTo("severity-low-high"), 
                "Should track reverse sort order");
        }

        [Test]
        public async Task GetSortedIssues_WithInvalidSortOrder_SetsSortOrderCorrectly()
        {
            // Act - Test handling of unknown sort parameter
            var result = await _controller.GetSortedIssues(1, "invalid-sort");

            // Assert - Verify fallback behavior
            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ViewBag.CurrentSort, Is.EqualTo("invalid-sort"), 
                "Should preserve invalid sort parameter");
        }
    }
}