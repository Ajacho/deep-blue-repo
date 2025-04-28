using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Text;
using System.Text.Json;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;
using System.Collections.Generic;

namespace Service_Tests
{
    [TestFixture]
    public class OpenAiService_Tests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private Mock<ILogger<OpenAiService>> _mockLogger;
        private OpenAiService _openAiService;

        [SetUp]
        public void Setup()
        {
            // Setup the mocked HttpMessageHandler.
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _mockLogger = new Mock<ILogger<OpenAiService>>();

            _openAiService = new OpenAiService(_httpClient, _mockLogger.Object);
        }

        [Test]
        public async Task AnalyzeWithOpenAI_NoIssuesFound_ReturnsEmptyString()
        {
            // Arrange: Setup a fake response that returns "No significant issues found" from the API.
            var fakeApiResponse = new OpenAiResponse
            {
                Choices = new List<Choice>
                {
                    new Choice { Message = new Message { Content = "No significant issues found" } }
                }
            };
            var fakeResponseJson = JsonSerializer.Serialize(fakeApiResponse);

            // Setup the HttpMessageHandler to return the fake response.
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponseJson, Encoding.UTF8, "application/json")
                });

            // Create a sample scrapedData dictionary.
            var scrapedData = new Dictionary<string, object>
            {
                { "headings", 5 },
                { "images", 10 },
                { "links", 8 },
                { "fonts", new List<string> { "Arial", "Roboto", "Verdana" } },
                { "text_content", "This is sample text for testing purposes." }
            };

            // Act
            var result = await _openAiService.AnalyzeWithOpenAI(
                "http://example.com", 
                "Test Category", 
                "Test Description", 
                scrapedData);

            // Assert: When "No significant issues found" is returned, the service should return an empty string.
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task AnalyzeWithOpenAI_IssuesFound_ReturnsAIResponseText()
        {
            // Arrange: Setup a fake response that returns an issue message.
            var fakeApiResponse = new OpenAiResponse
            {
                Choices = new List<Choice>
                {
                    new Choice { Message = new Message { Content = "Issue detected: Button is not accessible." } }
                }
            };
            var fakeResponseJson = JsonSerializer.Serialize(fakeApiResponse);

            // Setup the HttpMessageHandler to return the fake response.
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponseJson, Encoding.UTF8, "application/json")
                });

            // Create a sample scrapedData dictionary.
            var scrapedData = new Dictionary<string, object>
            {
                { "headings", 3 },
                { "images", 5 },
                { "links", 2 },
                { "fonts", new List<string> { "Helvetica", "Arial" } },
                { "text_content", "Sample text content." }
            };

            // Act
            var result = await _openAiService.AnalyzeWithOpenAI(
                "http://example.com", 
                "Test Category", 
                "Test Description", 
                scrapedData);

            // Assert: The returned response should match the content from the fake API response.
            Assert.That(result, Is.EqualTo("Issue detected: Button is not accessible."));
        }
        /* Test if FormatScrapedData function returns a formatted string */
        [Test]
        public void FormatScrapedData_ProducesExpectedOutput()
        {
            // Arrange: Create sample scraped data dictionary.
            var scrapedData = new Dictionary<string, object>
            {
                { "headings", 5 },
                { "images", 10 },
                { "links", 8 },
                { "fonts", new List<string> { "Arial", "Roboto", "Verdana" } },
                { "text_content", "This is sample text for testing purposes." }
            };

            // Build the expected formatted output using Environment.NewLine.
            string expected = string.Join(Environment.NewLine, new[]
            {
                "Headings Count: 5",
                "Images Count: 10",
                "Links Count: 8",
                "Fonts Used: Arial, Roboto, Verdana",
                "This is sample text for testing purposes.",
                "" // Represents the trailing newline.
            });

            // Act: Use reflection to invoke the private FormatScrapedData method.
            var method = _openAiService.GetType()
                .GetMethod("FormatScrapedData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(_openAiService, new object[] { scrapedData }) as string;

            // Assert: Ensure the result is not null or empty and matches the expected output.
            Assert.That(result, Is.Not.Null.And.Not.Empty, "FormatScrapedData returned null or empty string.");
            Assert.That(result.Trim(), Is.EqualTo(expected.Trim()), "The formatted output does not match the expected value.");
        }

        [Test]
        public async Task ImproveMessageAsync_ReturnsStringAsync()
        {
            // Arrange
            // Set up a mock service message and category to simulate an incoming request
            var mockServiceMessage = "This example has 5 headings, 10 images, 8 links, 3 fonts, and could be better";
            var mockCategory = "Broken Links";

            // Build a fake OpenAI API response
            var fakeApiResponse = new OpenAiResponse
            {
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Message = new Message
                        {
                            Content = "Our analysis found that the structure could be improved by adjusting headings, links, and images."
                        }
                    }
                }
            };

            // Serialize the fake API response into JSON format
            var fakeResponseJson = JsonSerializer.Serialize(fakeApiResponse);

            // Setup the mock HttpMessageHandler to intercept HTTP calls
            // When any HTTP POST request is sent, return a 200 OK response with the fake JSON
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    // Match any HTTP request
                    ItExpr.IsAny<HttpRequestMessage>(),
                    // Match any cancellation token
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK, // Simulate a successful OpenAI response
                    Content = new StringContent(fakeResponseJson, Encoding.UTF8, "application/json")
                });

            // Act
            // Call the method under test, which internally uses the mocked HTTP response
            var result = await _openAiService.ImproveMessageAsync(mockServiceMessage, mockCategory);

            // Assert
            // Validate that the result is a non-null, non-empty string
            Assert.That(result, Is.TypeOf<string>(), "Expected a string result from ImproveMessageAsync.");
            Assert.That(result, Is.Not.Null.And.Not.Empty, "The returned improved message should not be null or empty.");
            
            // Validate that the output has been improved 
            Assert.That(result, !Is.EqualTo(mockServiceMessage), "The improved message should be different from the raw input.");

            // Validate that the improved message starts with "Our analysis found" as per your writing standard
            Assert.That(result, Does.StartWith("Our analysis found"), "The improved message should begin with the required phrase.");
        }

        [Test]
        public async Task GenerateReportSummaryAsync_ReturnsStringAsync()
        {
            // Arrange
            // Prepare an empty list of design issues to simulate an edge case
            var mockIssues = new List<DesignIssue>();
            var mockUrl = "https://example.com";
            var mockHtml = "<html><body><h1>test</h1><p>this is a test</p><button>hi</button></body></html>";

            // Setup a fake OpenAI API response for the report summary
            var fakeApiResponse = new OpenAiResponse
            {
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        Message = new Message
                        {
                            Content = "Our analysis found that the site is generally well-structured but could improve accessibility features."
                        }
                    }
                }
            };

            var fakeResponseJson = JsonSerializer.Serialize(fakeApiResponse);

            // Mock the HttpMessageHandler to return the fake API response when called
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponseJson, Encoding.UTF8, "application/json")
                });

            // Act
            // Call the service method under test
            var result = await _openAiService.GenerateReportSummaryAsync(mockIssues, mockHtml, mockUrl);

            // Assert
            // The result should be a non-null, non-empty string
            Assert.That(result, Is.TypeOf<string>(), "Expected a string result from GenerateReportSummaryAsync.");
            Assert.That(result, Is.Not.Null.And.Not.Empty, "The generated report summary should not be null or empty.");
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }
    }

    // Helper classes to match the OpenAiResponse
    public class OpenAiResponse
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}