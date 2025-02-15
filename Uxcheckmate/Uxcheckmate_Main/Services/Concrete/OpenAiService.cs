using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public partial class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient; 
        private readonly ILogger<OpenAiService> _logger; 

        public OpenAiService(HttpClient httpClient, ILogger<OpenAiService> logger)
        {
            _httpClient = httpClient; 
            _logger = logger; 
        }

        public async Task<UxResult> AnalyzeUx(string url)
        {
            WebScraperService scraper = new WebScraperService(_httpClient);

            // Scrape the webpage content asynchronously
            Dictionary<string, object> scrapedData = await scraper.ScrapeAsync(url);

            // Convert scraped data into a readable format for AI analysis
            string pageContent = FormatScrapedData(scrapedData);

            var prompt = @$"Analyze the UX of the following webpage in structured sections:

            ### Fonts
            - How many unique fonts are used? Is this too many?
            - Is the typography consistent?

            ### Text Structure
            - Are there large blocks of text that need better separation?
            - Are headings used correctly for readability?

            ### Usability Issues
            - Are there too many links or images?
            - Are buttons, navigation, and layout elements easy to use?

            If no issues are found in a category, return 'No significant issues found' under that section.

            Webpage Data:
            {pageContent}";

            var request = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are a UX expert analyzing websites for accessibility, usability, and design flaws." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 500
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response into an OpenAiResponse object
            var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseString);

            // Extract the AI-generated content
            string aiText = openAiResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "No response";

            // Process extracted sections
            var sections = ExtractSections(aiText);

            // Convert structured data into a UxResult
            return ConvertToUxResult(sections);
        }

        // Extracts structured sections from AI-generated text output
        private Dictionary<string, string> ExtractSections(string aiResponse)
        {
            // Create a dictionary to store extracted sections
            var sections = new Dictionary<string, string>();

            // Regular expression pattern to find sections marked with "### SectionTitle"
            var regex = new System.Text.RegularExpressions.Regex(@"### (.*?)\n(.*?)(?=###|$)", System.Text.RegularExpressions.RegexOptions.Singleline);
            
            // Find all matches in the AI response
            var matches = regex.Matches(aiResponse);

            // Iterate through each matched section
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                // Extract the section title
                string sectionTitle = match.Groups[1].Value.Trim();

                // Extract the section content
                string sectionContent = match.Groups[2].Value.Trim();

                // Store the extracted section in the dictionary
                sections[sectionTitle] = sectionContent;
            }

            // Return the structured dictionary containing all sections
            return sections;
        }
        private UxResult ConvertToUxResult(Dictionary<string, string> sections)
        {
            var uxResult = new UxResult();

            foreach (var section in sections)
            {
                // Create a new UxIssue for each AI-generated category
                var issue = new UxIssue
                {
                    Category = section.Key, 
                    Message = section.Value, 
                    Selector = "" // 
                };

                uxResult.Issues.Add(issue);
            }

            return uxResult;
        }

        // Helper method to format the extracted webpage data into a readable text string
        private string FormatScrapedData(Dictionary<string, object> scrapedData)
        {
            var sb = new StringBuilder();

            sb.AppendLine("### UX Data Extracted from Web Page ###");

            // Add extracted numeric data (headings, images, and links count)
            sb.AppendLine($"- Headings Count: {scrapedData["headings"]}");
            sb.AppendLine($"- Images Count: {scrapedData["images"]}");
            sb.AppendLine($"- Links Count: {scrapedData["links"]}");

            // Extract and format font usage data
            var fonts = (List<string>)scrapedData["fonts"];
            sb.AppendLine($"- Fonts Used: {string.Join(", ", fonts)}"); // List all detected fonts
            sb.AppendLine($"- Total Unique Fonts: {fonts.Count}"); // Show the number of unique fonts used

            // Extract and format webpage text content
            string textContent = (string)scrapedData["text_content"];
            sb.AppendLine("\n### Extracted Page Text ###");

            // Limit displayed text content to 500 characters to avoid excessive output
            sb.AppendLine(textContent.Length > 500 ? textContent.Substring(0, 500) + "..." : textContent);

            // Return the formatted UX data string
            return sb.ToString();
        }

        [GeneratedRegex(@"###\s*(.*?)\s*\n(.*?)(?=(\n###|\z))", RegexOptions.Singleline)]
        private static partial Regex MyRegex();
    }
}
