using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using Uxcheckmate_Main.Models;
using System.Collections.Generic;
using System.IO;

namespace Uxcheckmate_Main.Services
{
    public class PdfExportService
    {
        public byte[] GenerateReportPdf(Report report)
        {
            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "uxCheckmateLogo.png");

            // return Document.Create(container =>
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(12));

                    // Unified Header
                    page.Header().Column(headerCol =>
                    {
                        // First Row: Website & Report Date + Logo
                        headerCol.Item().Row(headerRow =>
                        {
                            // Left-aligned Website & Report Date
                            headerRow.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Website: {report.Url}")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Grey.Darken2);

                                col.Item().Text($"Report Date: {report.Date}")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                            // Right-aligned Logo (if exists)
                            if (File.Exists(logoPath))
                            {
                                headerRow.ConstantItem(60).AlignRight().Height(40).Image(logoPath, ImageScaling.FitArea);
                            }
                        });

                        // Second Row: Centered Report Title
                        headerCol.Item().AlignCenter().Text("UXCheckmate Report")
                            .SemiBold()
                            .FontSize(24)
                            .FontColor(Colors.Blue.Medium);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        // Accessibility Issues Section
                        col.Item().Container().Background(Colors.Grey.Lighten3).Padding(10).Column(section =>
                        {
                            section.Item().Text("Accessibility Issues").Bold().FontSize(16);
                            if (report.AccessibilityIssues?.Count > 0)
                            {
                                section.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Issue").Bold();
                                        header.Cell().Text("Severity").Bold();
                                    });

                                    foreach (var issue in report.AccessibilityIssues)
                                    {
                                        table.Cell().Text(issue.Message);
                                        table.Cell().Text(issue.Severity.ToString()).FontColor(GetSeverityColor(issue.Severity));
                                    }
                                });
                            }
                            else
                            {
                                section.Item().Text("No accessibility issues found.").Italic();
                            }
                        });

                        // Design Issues Section
                        col.Item().Container().Background(Colors.Grey.Lighten4).Padding(10).Column(section =>
                        {
                            section.Item().Text("Design Issues").Bold().FontSize(16);
                            if (report.DesignIssues?.Count > 0)
                            {
                                section.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Issue").Bold();
                                        header.Cell().Text("Severity").Bold();
                                    });

                                    foreach (var issue in report.DesignIssues)
                                    {
                                        table.Cell().Text(issue.Message);
                                        table.Cell().Text(issue.Severity.ToString()).FontColor(GetSeverityColor(issue.Severity));
                                    }
                                });
                            }
                            else
                            {
                                section.Item().Text("No design issues found.").Italic();
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated by UXCheckmate - {System.DateTime.UtcNow}")
                        .FontSize(10).FontColor(Colors.Grey.Darken1);
                });
            });

        #if DEBUG
            // In development, show in Companion
            document.ShowInPreviewer();
            return null; // Nothing to return
        #else
            // In production, generate real PDF
            return document.GeneratePdf();
        #endif        

            // }).GeneratePdf();
            // use the following invocation
            // document.ShowInPreviewer();
        }

        private string GetSeverityColor(object severity)
        {
            return severity?.ToString()?.ToLower() switch
            {
                "high" => Colors.Red.Darken2,
                "medium" => Colors.Orange.Darken2,
                "low" => Colors.Green.Darken2,
                _ => Colors.Black
            };
        }
    }
}
