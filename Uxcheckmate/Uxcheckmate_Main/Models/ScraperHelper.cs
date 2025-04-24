using System;
using System.Collections.Generic;

namespace Uxcheckmate_Main.Models
{
    public class ScraperHelper
    {
        public string htmlContent { get; set; }
        public int headings { get; set; }
        public int paragraphs { get; set; }
        public int images { get; set; }
        public List<string> links { get; set; }
        public string text_content { get; set; }
        public List<string> fonts { get; set; }
        public bool hasFavicon { get; set; }
        public string faviconUrl { get; set; }
        public double scrollHeight { get; set; }
        public double viewportHeight { get; set; }
        public double scrollWidth { get; set; }
        public double viewportWidth { get; set; }
        public string viewportLabel { get; set; }
        public List<string> inlineCssList { get; set; }
        public List<string> inlineJsList { get; set; }
        public List<string> externalCssLinks { get; set; }
        public List<string> externalJsLinks { get; set; }
    }
}
