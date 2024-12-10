using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class ApplicantMerit
{
    public long Id { get; set; }

    public long? ApplicantId { get; set; }

    public string? Name { get; set; }

    public virtual Applicant? Applicant { get; set; }
}
