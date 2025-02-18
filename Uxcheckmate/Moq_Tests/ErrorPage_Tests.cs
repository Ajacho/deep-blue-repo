using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit;
using Uxcheckmate_Main.Controllers;

namespace Moq_Tests.ErrorPage_Tests
{
    public class ErrorPage_Tests
    {
        private Mock<ILogger<HomeController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
        }

        [Test]
        public void Error404_ReturnsErrorPageView()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);

            // Act
            var result = controller.ErrorPage() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("ErrorPage"));
        }
    }
}
