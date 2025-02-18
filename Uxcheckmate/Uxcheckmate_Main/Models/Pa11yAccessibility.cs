using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Uxcheckmate_Main.Models;

public class Pa11yIssue
{
     //  If the case of the JSON keys differs from your model’s property names
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("selector")]
    public string Selector { get; set; }
}


public class Pa11yResult{
        public List<Pa11yIssue>? Issues { get; set; }
}