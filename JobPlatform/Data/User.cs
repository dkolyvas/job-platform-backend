using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class User
{
    public long Id { get; set; }

    public string? Username { get; set; } 

    public string? Password { get; set; }

    public string? Email { get; set; }
    
    public string? Role { get; set; }

    public int? UnauthorizedCount { get; set; }

    public string? RestoreCode { get; set; }

    public virtual Applicant? Applicant { get; set; }

    public virtual Business? Business { get; set; }
}
