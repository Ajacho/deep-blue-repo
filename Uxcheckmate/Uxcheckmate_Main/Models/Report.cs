using System;
using System.Collections.Generic;

namespace Uxcheckmate_Main.Models;

public partial class Report
{
    public int ReportId { get; set; }
    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public DateTime Date { get; set; }

    public string Recommendations { get; set; } = null!;

    public virtual ReportCategory Category { get; set; } = null!;

    public virtual UserAccount? User { get; set; } 
}
