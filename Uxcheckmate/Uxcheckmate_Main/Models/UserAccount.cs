using System;
using System.Collections.Generic;

namespace Uxcheckmate_Main.Models;

public partial class UserAccount
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual Role Role { get; set; } = null!;
}
