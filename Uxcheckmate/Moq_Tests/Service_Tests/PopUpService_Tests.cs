using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Uxcheckmate_Main.Services;

namespace Service_Tests
{
    [TestFixture]
    public class PopUpsServiceTests
    {
        private Mock<ILogger<PopUpsService>> _mockLogger;
        private PopUpsService _service;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<PopUpsService>>();
            _service = new PopUpsService(_mockLogger.Object);
        }

        [Test]
        public async Task AnalyzePopupsAsync_MultiplePopups_ReturnsWarning()
        {
            // Arrange
            var html = @"
                <div class='popup'></div>
                <div role='dialog'></div>
            ";
            var jsList = new List<string> { "function openModal() { /* show popup */ }" };

            // Act
            var result = await _service.AnalyzePopupsAsync(html, jsList);

            // Assert
            Assert.That(result, Does.Contain("More than 1 Pop Up detected"));
        }

        [Test]
        public async Task AnalyzePopupsAsync_OnePopup_ReturnsEmpty()
        {
            // Arrange
            var html = "<div class='popup'></div>";
            var jsList = new List<string>();

            // Act
            var result = await _service.AnalyzePopupsAsync(html, jsList);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }
    }
}
