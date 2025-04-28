using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public interface IOpenAiService

    { 
        Task<string> AnalyzeWithOpenAI(string url, string categoryName, string categoryDescription, Dictionary<string, object> scrapedData);
        Task<string> ImproveMessageAsync(string rawMessage, string categoryName);
        Task<string> GenerateReportSummaryAsync(List<DesignIssue> issues, string html, string url);
    }
}