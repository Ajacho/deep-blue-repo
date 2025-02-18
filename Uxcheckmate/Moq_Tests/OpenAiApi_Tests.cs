using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Uxcheckmate_Main.Services;

namespace Moq_Tests.OpenAiApi_Tests
{
    [TestFixture]
    public class OpenAiApi_Tests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private Mock<ILogger<OpenAiService>> _loggerMock;
        private OpenAiService _openAiService;

        [SetUp]
        public void Setup()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _loggerMock = new Mock<ILogger<OpenAiService>>();

            _openAiService = new OpenAiService(_httpClient, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose HttpClient after each test
            _httpClient.Dispose();
        }

        [Test]
        public async Task AnalyzeUx_CallsOpenAiAndProcessesResponse()
        {
            // Arrange: Mock OpenAI API Response
            var mockResponse = new
            {
                choices = new[]
                {
                    new { message = new { content = "### Fonts\n- Too many fonts used.\n### Text Structure\n- Large text blocks found." } }
                }
            };

            var responseJson = JsonSerializer.Serialize(mockResponse);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act: Call the AnalyzeUx method
            var result = await _openAiService.AnalyzeUx("https://example.com");

            // Assert: Verify that the OpenAI response was correctly processed
            Assert.That(result, Is.Not.Null, "AnalyzeUx returned null.");
            Assert.That(result.Issues, Has.Count.EqualTo(2), "Expected 2 UX issues.");
            Assert.That(result.Issues[0].Category, Is.EqualTo("Fonts"), "First issue category mismatch.");
            Assert.That(result.Issues[0].Message, Is.EqualTo("- Too many fonts used."), "First issue message mismatch.");
            Assert.That(result.Issues[1].Category, Is.EqualTo("Text Structure"), "Second issue category mismatch.");
            Assert.That(result.Issues[1].Message, Is.EqualTo("- Large text blocks found."), "Second issue message mismatch.");
        }
    }
}