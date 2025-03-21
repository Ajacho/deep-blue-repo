using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Uxcheckmate_Main.Models;
using System.Collections.Generic;
using System.IO;

namespace Uxcheckmate_Main.Services
{
    public class PdfExportService
    {
        public byte[] GenerateReportPdf(Report report)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header()
                        .Text($"UXCheckmate Report for {report.Url}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text($"Report Date: {report.Date}");

                        col.Item().Text("Accessibility Issues").Bold();
                        if (report.AccessibilityIssues?.Count > 0)
                        {
                            foreach (var issue in report.AccessibilityIssues)
                            {
                                col.Item().Text($"• {issue.Message} (Severity: {issue.Severity})");
                            }
                        }
                        else
                        {
                            col.Item().Text("No accessibility issues found.").Italic();
                        }

                        col.Item().Text("Design Issues").Bold();
                        if (report.DesignIssues?.Count > 0)
                        {
                            foreach (var issue in report.DesignIssues)
                            {
                                col.Item().Text($"• {issue.Message} (Severity: {issue.Severity})");
                            }
                        }
                        else
                        {
                            col.Item().Text("No design issues found.").Italic();
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated by UXCheckmate - {System.DateTime.UtcNow}");
                });
            }).GeneratePdf();
        }
    }
}