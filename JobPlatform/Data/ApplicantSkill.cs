using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class ApplicantSkill
{
    public long Id { get; set; }

    public long ApplicantId { get; set; }

    public long SkillSubcategoryId { get; set; }

    public int? SkillLevelId { get; set; }

    public string? Institution { get; set; }

    public string? Description { get; set; }

    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }

    public int? DurationMonths { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;

    public virtual SkillLevel? SkillLevel { get; set; }

    public virtual SkillSubcategory SkillSubcategory { get; set; } = null!;
}
