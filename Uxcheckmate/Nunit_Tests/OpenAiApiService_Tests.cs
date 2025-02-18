using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Uxcheckmate_Main.Services;
using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Tests.Services
{
    [TestFixture]
    public class OpenAiServiceUnitTests
    {
        private OpenAiService _openAiService;

        [SetUp]
        public void Setup()
        {
            _openAiService = new OpenAiService(null, null); 
        }

        /* Test if FormatScrapedData function returns a formatted string */
        [Test]
        public void FormatScrapedData_ReturnFormattedString()
        {
            // Arrange
            var scrapedData = new Dictionary<string, object>
            {
                { "headings", 5 },
                { "images", 10 },
                { "links", 8 },
                { "fonts", new List<string> { "Arial", "Roboto", "Verdana" } },
                { "text_content", "This is sample text for testing purposes." }
            };

            // Act
            var result = _openAiService.GetType()
                .GetMethod("FormatScrapedData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_openAiService, new object[] { scrapedData }) as string;

            // Assert
            Assert.That(result, Is.Not.Null.And.Not.Empty, "FormatScrapedData returned null or empty string.");

            Assert.That(result, Does.Contain("Headings Count: 5"), "Missing 'Headings Count: 5' in formatted data.");
            Assert.That(result, Does.Contain("Images Count: 10"), "Missing 'Images Count: 10' in formatted data.");
            Assert.That(result, Does.Contain("Links Count: 8"), "Missing 'Links Count: 8' in formatted data.");
            Assert.That(result, Does.Contain("Fonts Used: Arial, Roboto, Verdana"), "Missing font list in formatted data.");
            Assert.That(result, Does.Contain("This is sample text for testing purposes."), "Missing text content in formatted data.");
        }

        /* Test if ExtractSections function returns a dictionary */
        [Test]
        public void ExtractSections_ReturnsDictionary()
        {
            // Arrange
            string aiResponse = @"
                ### Fonts
                - Too many fonts used.

                ### Text Structure
                - Large text blocks found.

                ### Usability Issues
                - No significant issues found.";

            // Act
            var result = _openAiService.GetType()
                .GetMethod("ExtractSections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_openAiService, new object[] { aiResponse }) as Dictionary<string, string>;

            // Debugging Output
            Console.WriteLine("\nExtracted Sections:");
            if (result != null)
            {
                foreach (var entry in result)
                {
                    Console.WriteLine($"Category: {entry.Key}\nContent: {entry.Value}\n");
                }
            }
            else
            {
                Console.WriteLine("ExtractSections returned NULL");
            }

            // Assert
            Assert.That(result, Is.Not.Null, "ExtractSections returned null.");
            Assert.That(result.Count, Is.EqualTo(3), $"Expected 3 sections, but got {result?.Count ?? 0}");
            Assert.That(result["Fonts"], Is.EqualTo("- Too many fonts used."));
            Assert.That(result["Text Structure"], Is.EqualTo("- Large text blocks found."));
            Assert.That(result["Usability Issues"], Is.EqualTo("- No significant issues found."));
        }

        /* Tests if ConvertToUxResult returns a UxResult */
        [Test]
        public void ConvertToUxResult_ReturnsUxResult()
        {
            // Arrange
            var sections = new Dictionary<string, string>
            {
                { "Fonts", "Too many fonts used." },
                { "Text Structure", "Large text blocks found." }
            };

            // Act
            var result = _openAiService.GetType()
                .GetMethod("ConvertToUxResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_openAiService, new object[] { sections }) as UxResult;

            // Assert (NUnit 3+)
            Assert.That(result, Is.Not.Null, "ConvertToUxResult returned null.");
            Assert.That(result.Issues, Has.Count.EqualTo(2), "Expected 2 UX issues.");
            
            Assert.That(result.Issues[0].Category, Is.EqualTo("Fonts"), "First issue category mismatch.");
            Assert.That(result.Issues[0].Message, Is.EqualTo("Too many fonts used."), "First issue message mismatch.");
            
            Assert.That(result.Issues[1].Category, Is.EqualTo("Text Structure"), "Second issue category mismatch.");
            Assert.That(result.Issues[1].Message, Is.EqualTo("Large text blocks found."), "Second issue message mismatch.");
        }
    }
}
