using Uxcheckmate_Main.Models;

namespace Uxcheckmate_Main.Services
{
    public interface IOpenAiService

    { 
        public Task<Dictionary<string, string>> AnalyzeUx(string url);
    }
}