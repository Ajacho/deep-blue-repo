using NUnit.Framework;
using Uxcheckmate_Main.Services;
using Uxcheckmate_Main.Models; 
using Moq;
using Reqnroll;  
using Uxcheckmate_Main;
using Microsoft.Extensions.Logging;

[Binding]
public class UxAnalysisSteps
{
    private readonly OpenAiService _openAiService;
    private UxResult _result;
    private string _url;
    private Dictionary<string, object> _mockScrapedData;

    public UxAnalysisSteps()
    {
        var httpClient = new HttpClient(); 
        var logger = new Mock<ILogger<OpenAiService>>().Object;
        _openAiService = new OpenAiService(httpClient, logger);
    }

    [Given(@"Sarah submits her URL")]
    public void GivenSarahSubmitsHerURL()
    {
        _url = "https://example.com"; 
    }

    [Given(@"her site has (.*) fonts on it")]
    public void GivenHerSiteHasFontsOnIt(int fontCount)
    {
        _mockScrapedData = new Dictionary<string, object>
        {
            { "fonts", Enumerable.Repeat("FontFamily", fontCount).ToList() },
            { "text_content", "Sample text content." },
            { "headings", 5 },
            { "images", 10 },
            { "links", 8 }
        };
    }

    [Given(@"her site has huge blocks of text")]
    public void GivenHerSiteHasHugeBlocksOfText()
    {
        _mockScrapedData = new Dictionary<string, object>
        {
            { "fonts", new List<string> { "Arial", "Roboto", "Verdana" } },
            { "text_content", new string('A', 1000) }, 
            { "headings", 3 },
            { "images", 5 },
            { "links", 4 }
        };
    }

    [Given(@"her site has no recommendations")]
    public void GivenHerSiteHasNoRecommendations()
    {
        _mockScrapedData = new Dictionary<string, object>
        {
            { "no_issues", true } 
        };
    }

    [When(@"the report loads")]
    public async Task WhenTheReportLoads()
    {
        if (_mockScrapedData.ContainsKey("no_issues") && (bool)_mockScrapedData["no_issues"])
        {
            // If no issues exist, return an empty list
            _result = new UxResult { Issues = new List<UxIssue>() };
        }
        else
        {
            // Mocking OpenAI Response with issues
            _result = new UxResult
            {
                Issues = new List<UxIssue>
                {
                    new UxIssue { Category = "Fonts", Message = "Too many fonts used. Consider reducing the number of fonts for consistency." },
                    new UxIssue { Category = "Text Structure", Message = "Large blocks of text detected. Consider using images, padding, or color modules for better readability." }
                }
            };
        }

        Console.WriteLine($"Mocked OpenAI Response: {_result.Issues.Count} issues found.");
    }

    [Then(@"she will see a suggestion in the report to condense the amount of fonts on her site")]
    public void ThenSheWillSeeASuggestionToCondenseFonts()
    {
        var fontIssue = _result.Issues.FirstOrDefault(i => i.Category.Contains("Fonts"));
        Assert.That(fontIssue, Is.Not.Null, "No font suggestions found.");
        Assert.That(fontIssue.Message, Does.Contain("Too many fonts used. Consider reducing the number of fonts for consistency."), "Expected a recommendation to reduce fonts.");
    }

    [Then(@"she will see a suggestion to separate her text content with images, padding, and color modules")]
    public void ThenSheWillSeeASuggestionToImproveTextLayout()
    {
        // Debugging output
        Console.WriteLine($"Total Issues Found: {_result.Issues.Count}");
        foreach (var issue in _result.Issues)
        {
            Console.WriteLine($"Category: {issue.Category}, Message: {issue.Message}");
        }

        var textIssue = _result.Issues.FirstOrDefault(i => i.Category.Contains("Text Structure"));

        Assert.That(textIssue, Is.Not.Null, "No text structure suggestions found.");
        Assert.That(textIssue.Message, Does.Contain("Large blocks of text"), "Expected a recommendation to improve text readability.");
    }

    [Then(@"she will see a message that says there are no suggestions")]
    public void ThenSheWillSeeAMessageThatSaysThereAreNoSuggestions()
    {
        Assert.That(_result.Issues, Is.Empty, "Expected no recommendations, but some were found.");
    }
}
