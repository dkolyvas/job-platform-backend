using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class Applicant
{
    public long Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public long? UserId { get; set; }

    public string? Cv { get; set; }

    public virtual ICollection<ApplicantMerit> ApplicantMerits { get; set; } = new List<ApplicantMerit>();

    public virtual ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new List<ApplicantSkill>();

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual User? User { get; set; }
}
