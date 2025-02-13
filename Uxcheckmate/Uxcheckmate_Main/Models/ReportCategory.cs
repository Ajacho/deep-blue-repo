using System;
using System.Collections.Generic;

namespace Uxcheckmate_Main.Models;

public partial class ReportCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string OpenAiprompt { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
