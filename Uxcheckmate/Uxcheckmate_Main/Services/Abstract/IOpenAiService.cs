using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public interface IOpenAiService

    { 
        public Task<List<Report>> AnalyzeAndSaveUxReports(string url);
    }
}