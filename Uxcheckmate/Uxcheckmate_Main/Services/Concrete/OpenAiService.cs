using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient; 
        private readonly ILogger<OpenAiService> _logger; 

        public OpenAiService(HttpClient httpClient, ILogger<OpenAiService> logger)
        {
            _httpClient = httpClient; 
            _logger = logger; 
        }

        public async Task<string> AnalyzeUx(string url)
        {
            // Initialize the WebScraperService to extract content from the given URL
            WebScraperService scraper = new WebScraperService(_httpClient);
            
            // Scrape the webpage content asynchronously
            string pageContent = await scraper.ScrapeAsync(url);

            // Prompt
            var prompt = $"Analyze the UX of the following webpage and provide recommendations for improvements based on accessibility, usability, and best design practices:\n\n{pageContent}";
            // Possible Prompts: Analyze the design of the following webpage and make recommendations for improvement in regards to fonts on the page: \n\n{pageContent}
            // Possible Prompts: Analyze the ux design of the following webpage and provide recommendations for improvement in regards to chunks of text on a page: \n\n{pageContent}
            
            // Payload Request
            var request = new
            {
                model = "gpt-4", // AI Model
                messages = new[]
                {
                    new { role = "system", content = "You are a UX expert analyzing websites for accessibility, usability, and design flaws." },
                    new { role = "user", content = prompt } // User input containing the UX analysis request
                },
                max_tokens = 200 // Limit the response length to 200 tokens
            };

            // Convert the request object into a JSON payload with appropriate encoding
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            // Send the request to OpenAI's chat completion API endpoint
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);

            // Read the API response content as a string
            var responseString = await response.Content.ReadAsStringAsync();

            // Return the AI-generated UX recommendations
            return responseString;
        }
    }
}