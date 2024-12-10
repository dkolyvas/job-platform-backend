using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class SkillLevel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? SkillSort { get; set; }

    public int? SkillCategoryId { get; set; }

    public long? SkillSubcategoryId { get; set; }

    public int? Grade { get; set; } = 0;

    public virtual ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new List<ApplicantSkill>();

    public virtual SkillSubcategory? SkillSubcategory { get; set; }

    public virtual SkillCategory? SkillCategory { get; set; }

    public virtual ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
}
