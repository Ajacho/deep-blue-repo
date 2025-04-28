using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Uxcheckmate_Main.Models;

[Table("Report")]
public partial class Report
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("URL")]
    [StringLength(128)]
    [Unicode(false)]
    public string Url { get; set; } = null!;

    public DateOnly Date { get; set; }

    public string? Summary { get; set; }
    [Column("UserID")]
    public string? UserID { get; set; }

    [InverseProperty("Report")]
    public virtual ICollection<AccessibilityIssue> AccessibilityIssues { get; set; } = new List<AccessibilityIssue>();

    [InverseProperty("Report")]
    public virtual ICollection<DesignIssue> DesignIssues { get; set; } = new List<DesignIssue>();
}
