using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Uxcheckmate_Main.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Playwright;
using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Tests
{
    public class LayoutParsingServiceTests
    {
        private LayoutParsingService _service;
        [SetUp]
        public void Setup()
        {
            _service = new LayoutParsingService(new NullLogger<LayoutParsingService>());
        }
        [Test]
        public async Task ExtractHtmlElementsAsync_ReturnsParsedElements()
        {
            // Mocked Json
            string json = @"
            [
                { ""tag"": ""H1"", ""x"": 100, ""y"": 50, ""width"": 500, ""height"": 80, ""text"": ""Header"" },
                { ""tag"": ""P"", ""x"": 100, ""y"": 150, ""width"": 800, ""height"": 100, ""text"": ""Paragraph"" }
            ]";
            var doc = JsonDocument.Parse(json);
            var jsResult = doc.RootElement;
            var page = new Mock<IPage>();
            page.Setup(p => p.EvaluateAsync<JsonElement>(It.IsAny<string>(), It.IsAny<object?>())).ReturnsAsync(jsResult);
            var elements = await _service.ExtractHtmlElementsAsync(page.Object);
            Assert.That(elements, Is.Not.Null);
            Assert.That(elements.Count, Is.EqualTo(2));
            Assert.That(elements[0].Tag, Is.EqualTo("H1"));
            Assert.That(elements[0].X, Is.EqualTo(100));
        }
        [Test]
        public async Task ExtractHtmlElementsAsync_OnError_ReturnsEmptyList()
        {
            var page = new Mock<IPage>();
            page.Setup(p => p.EvaluateAsync<JsonElement>(It.IsAny<string>(), It.IsAny<object?>())).ThrowsAsync(new System.Exception("JS execution failed"));
            var result = await _service.ExtractHtmlElementsAsync(page.Object);
            Assert.That(result, Is.Empty);
        }
    }
}
