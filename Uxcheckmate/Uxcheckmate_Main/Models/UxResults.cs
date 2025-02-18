using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Uxcheckmate_Main.Models
{
    public class UxIssue
    {
        public string? Category { get; set; } 
        public string? Message { get; set; }
        public string? Selector { get; set; } 
    }
        public class OpenAiResponse
        {
            [JsonPropertyName("choices")]
            public List<Choice>? Choices { get; set; }
        }

        public class Choice
        {
            [JsonPropertyName("message")]
            public AiMessage Message { get; set; }
        }

        public class AiMessage
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }
    public class UxResult
    {
        public List<UxIssue> Issues { get; set; } = new List<UxIssue>();
    }
}
