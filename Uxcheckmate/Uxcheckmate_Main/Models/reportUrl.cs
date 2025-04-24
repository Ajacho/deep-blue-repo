using System.ComponentModel.DataAnnotations;

namespace Uxcheckmate_Main.Models
{
    public class ReportUrl
    {
        [Required(ErrorMessage = "Please enter a URL")]
        // [Url(ErrorMessage = "Please enter a valid URL")]
        public string? Url { get; set; }
        public int Headings { get; set; }
        public int Images { get; set; }
        public string ? Base64Screenshot { get; set; }
    }
}