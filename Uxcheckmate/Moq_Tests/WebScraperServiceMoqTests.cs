using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Uxcheckmate_Main.Services;

[TestFixture]
public class WebScraperServiceMockTests
{
    private Mock<HttpMessageHandler> _handlerMock;
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object) ?? throw new System.Exception("HttpClient initialization failed");
    }

    [TearDown]
    public void Cleanup()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task FetchHtmlAsync_ShouldReturnEmptyHtmlContent_MarinaOfficial()
    {
        // Arrange
        var expectedUrl = "https://marinaofficial.co.uk/";
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });

        var scraperService = new WebScraperService(_httpClient);

        // Act
        var result = await scraperService.FetchHtmlAsync(expectedUrl);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty), "Expected empty HTML content but got something else.");
    }

    [Test]
    public async Task FetchHtmlAsync_ShouldReturnHtmlContent_WouEdu()
    {
        // Arrange
        var expectedUrl = "https://wou.edu/";
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("<html><body>Mocked Content</body></html>")
            });

        var scraperService = new WebScraperService(_httpClient);

        // Act
        var result = await scraperService.FetchHtmlAsync(expectedUrl);

        // Assert
        Assert.That(result, Is.Not.Empty, "Expected non-empty HTML content but got empty string.");
        Assert.That(result, Is.EqualTo("<html><body>Mocked Content</body></html>"), "HTML content mismatch.");
    }

    [Test]
    public async Task ScrapeAsync_ShouldReturnCorrectElementCounts()
    {
        // Arrange
        var expectedUrl = "https://example.com/";
        var mockHtml = "<html><body>" +
                       "<h2>Heading 1</h2><h2>Heading 2</h2>" +
                       "<img src='img1.jpg'/><img src='img2.jpg'/><img src='img3.jpg'/>" +
                       "<a href='link1.html'></a><a href='link2.html'></a>" +
                       "</body></html>";

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockHtml)
            });

        var scraperService = new WebScraperService(_httpClient);

        // Act
        var result = await scraperService.ScrapeAsync(expectedUrl);

        // Debugging output
        TestContext.WriteLine($"Scraped {result["headings"]} headings.");
        TestContext.WriteLine($"Scraped {result["images"]} images.");
        TestContext.WriteLine($"Scraped {result["links"]} links.");

        // Assert expected counts
        Assert.That(result["headings"], Is.EqualTo(2), "Incorrect number of headings scraped.");
        Assert.That(result["images"], Is.EqualTo(3), "Incorrect number of images scraped.");
        Assert.That(result["links"], Is.EqualTo(2), "Incorrect number of links scraped.");
    }
}
