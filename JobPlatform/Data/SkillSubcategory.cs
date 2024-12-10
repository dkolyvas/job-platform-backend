using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class SkillSubcategory
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public int SkillCategoryId { get; set; }

    public bool? Checked { get; set; }

    public virtual ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new List<ApplicantSkill>();

    public virtual SkillCategory SkillCategory { get; set; } = null!;

    public virtual ICollection<SkillLevel> SkillLevels { get; set; } = new List<SkillLevel>();

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();

    public virtual ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
}
