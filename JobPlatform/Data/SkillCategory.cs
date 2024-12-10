using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class SkillCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int Sort { get; set; }

    public bool? Checked { get; set; }

    public virtual ICollection<SkillSubcategory> SkillSubcategories { get; set; } = new List<SkillSubcategory>();

    public virtual ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
    public virtual ICollection<SkillLevel> SkillLevels { get; set; } = new List<SkillLevel>();
}
