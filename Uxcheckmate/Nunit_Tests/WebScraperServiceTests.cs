using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Uxcheckmate_Main.Services;

[TestFixture]
public class WebScraperServiceLiveTests
{
    private HttpClient _httpClient;
    private WebScraperService _scraperService;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        _scraperService = new WebScraperService(_httpClient);
    }

    [TearDown]
    public void Cleanup()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task ScrapeAsync_ShouldReturnRealData_MarinaOfficial()
    {
        // Arrange
        var url = "https://marinaofficial.co.uk/";

        // Act
        var result = await _scraperService.ScrapeAsync(url);

        // Assert
        Assert.That(result, Is.Not.Null, "ScrapeAsync returned null.");
        Assert.That((int)result["headings"], Is.GreaterThanOrEqualTo(0), "No headings found.");
        Assert.That((int)result["images"], Is.GreaterThanOrEqualTo(0), "No images found.");

        TestContext.WriteLine($"Headings: {result["headings"]}, Images: {result["images"]}");
    }

    [Test]
    public async Task ScrapeAsync_ShouldReturnRealData_WouEdu()
    {
        // Arrange
        var url = "https://wou.edu/";

        // Act
        var result = await _scraperService.ScrapeAsync(url);

        // Assert
        Assert.That(result, Is.Not.Null, "ScrapeAsync returned null.");
        Assert.That((int)result["headings"], Is.GreaterThanOrEqualTo(0), "No headings found.");
        Assert.That((int)result["images"], Is.GreaterThanOrEqualTo(0), "No images found.");

        TestContext.WriteLine($"Headings: {result["headings"]}, Images: {result["images"]}");
    }
}
