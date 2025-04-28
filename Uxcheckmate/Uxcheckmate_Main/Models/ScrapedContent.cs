using System;
using System.Collections.Generic;

namespace Uxcheckmate_Main.Models
{
    public class ScrapedContent
    {
        public string Url { get; set; }
        public string HtmlContent { get; set; }

        public int Headings { get; set; }
        public int Paragraphs { get; set; }
        public int Images { get; set; }

        public List<string> Links { get; set; }
        public string TextContent { get; set; }

        public List<string> Fonts { get; set; }

        public bool HasFavicon { get; set; }
        public string FaviconUrl { get; set; }

        public double ScrollHeight { get; set; }
        public double ViewportHeight { get; set; }
        public double ScrollWidth { get; set; }
        public double ViewportWidth { get; set; }
        public string ViewportLabel { get; set; }

        public string InlineCss { get; set; }
        public string InlineJs { get; set; }

        public List<string> InlineCssList { get; set; }
        public List<string> InlineJsList { get; set; }
        public List<string> ExternalCssLinks { get; set; }
        public List<string> ExternalJsLinks { get; set; }
        public List<string> ExternalCssContents { get; set; }
        public List<string> ExternalJsContents { get; set; }
        public List<HtmlElement> LayoutElements { get; set; } = new();


        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "url", Url },
                { "htmlContent", HtmlContent },
                { "headings", Headings },
                { "paragraphs", Paragraphs },
                { "images", Images },
                { "links", Links },
                { "text_content", TextContent },
                { "fonts", Fonts },
                { "hasFavicon", HasFavicon },
                { "faviconUrl", FaviconUrl },
                { "scrollHeight", ScrollHeight },
                { "viewportHeight", ViewportHeight },
                { "scrollWidth", ScrollWidth },
                { "viewportWidth", ViewportWidth },
                { "viewportLabel", ViewportLabel },
                { "inlineCss", InlineCss },
                { "inlineJs", InlineJs },
                { "inlineCssList", InlineCssList },
                { "inlineJsList", InlineJsList },
                { "externalCssLinks", ExternalCssLinks },
                { "externalJsLinks", ExternalJsLinks },
                { "externalCssContents", ExternalCssContents },
                { "externalJsContents", ExternalJsContents }
            };
        }
    }
}
