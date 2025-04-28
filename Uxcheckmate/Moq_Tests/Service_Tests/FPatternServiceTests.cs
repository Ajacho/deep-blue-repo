using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Uxcheckmate_Tests
{
    public class FPatternServiceTests
    {
        private FPatternService _service;
        [SetUp]
        public void Setup()
        {
            _service = new FPatternService(new NullLogger<FPatternService>());
        }
        [Test]
        public async Task ProperlyBalancedFPattern()
        {
            double width = 1000, height = 1000;
            var elements = new List<HtmlElement>
            {
                new HtmlElement { Tag = "H1", X = 100, Y = 50, Width = 300, Height = 50, Text = "Header" },
                new HtmlElement { Tag = "P", X = 100, Y = 200, Width = 500, Height = 50, Text = "Paragraph text" },
                new HtmlElement { Tag = "IMG", X = 100, Y = 500, Width = 300, Height = 200, Text = "" }
            };
            var result = await _service.AnalyzeFPatternAsync(width, height, elements);
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public async Task ImproperlyBalancedFPattern()
        {
            double width = 1000, height = 1000;
            var elements = new List<HtmlElement>
            {
                new HtmlElement { Tag = "H1", X = 900, Y = 700, Width = 300, Height = 50, Text = "Header text with a lot of length" },
                new HtmlElement { Tag = "P", X = 950, Y = 950, Width = 300, Height = 200, Text = "More heavy text very far from F pattern zones" }
            };
            var result = await _service.AnalyzeFPatternAsync(width, height, elements);
            Assert.That(result, Does.Contain("This layout does not follow the F-pattern well."));
        }
        [Test]
        public async Task NoElementsReturnsNoElementsWarning()
        {
            var result = await _service.AnalyzeFPatternAsync(1000, 1000, new List<HtmlElement>());
            Assert.That(result, Does.Contain("No layout elements were found"));
        }
        [Test]
        public async Task BandSplittingWorksWithGaps()
        {
            double width = 1000, height = 2000;
            var elements = new List<HtmlElement>
            {
                new HtmlElement { Tag = "H1", X = 100, Y = 50, Width = 300, Height = 50, Text = "Header1" },
                new HtmlElement { Tag = "P", X = 100, Y = 250, Width = 300, Height = 100, Text = "Text1" },
                new HtmlElement { Tag = "H2", X = 100, Y = 900, Width = 300, Height = 50, Text = "Header2" },
                new HtmlElement { Tag = "P", X = 100, Y = 1100, Width = 300, Height = 100, Text = "Text2" }
            };
            var result = await _service.AnalyzeFPatternAsync(width, height, elements);
            Assert.That(result, Is.EqualTo("")); // Both bands should be above 0.5 in score
        }
    }
}
