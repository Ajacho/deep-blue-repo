using Uxcheckmate_Main.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Uxcheckmate_Main.Services
{
    public interface IPa11yService
    {
      Task<List<Pa11yIssue>> AnalyzeAndSaveAccessibilityReport(string url);
    }
}