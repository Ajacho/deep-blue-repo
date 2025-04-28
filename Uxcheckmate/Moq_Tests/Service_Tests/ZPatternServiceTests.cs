using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;
using Uxcheckmate_Main.Services;
using Uxcheckmate_Main.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq_Tests;

namespace Uxcheckmate_Tests
{
    public class ZPatternServiceTests
    {
        private ZPatternService _service;
        [SetUp]
        public void Setup()
        {
            var loggerMock = new Mock<ILogger<ZPatternService>>();
            _service = new ZPatternService(loggerMock.Object);
        }
        [Test]
        public async Task AnalyzeZPatternAsyncGoodZLayout()
        {
            var elements = new List<HtmlElement>
            {
                new TestHtmlElement(50, 30, 100, 20, 0.2),   // Top left
                new TestHtmlElement(700, 40, 100, 20, 0.2),  // Top right
                new TestHtmlElement(300, 300, 80, 20, 0.15), // Mid-diagonal
                new TestHtmlElement(500, 500, 80, 20, 0.15), // Mid-diagonal lower
                new TestHtmlElement(50, 750, 150, 30, 0.2),  // Bottom left
                new TestHtmlElement(700, 770, 100, 20, 0.2)  // Bottom right
            };
            var result = await _service.AnalyzeZPatternAsync(800, 800, elements);
            Assert.That(result, Is.Empty, "Expected no warnings for a strong Z pattern layout.");
        }
        [Test]
        public async Task AnalyzeZPatternAsyncBadLayout()
        {
            var elements = new List<HtmlElement>
            {
                new TestHtmlElement(400, 400, 300, 300, 0.02) // Only center
            };
            var result = await _service.AnalyzeZPatternAsync(800, 800, elements);
            Assert.That(result, Does.Contain("does not follow the Z-pattern well"), "Expected warning message for bad Z pattern layout.");
        }
    }
}
