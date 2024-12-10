using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class VacancySkill
{
    public long Id { get; set; }

    public long VacancyId { get; set; }

    public int SkillSort { get; set; }

    public int? SkillCategoryId { get; set; }

    public long? SkillSubcategoryId { get; set; }

    public int? SkillLevelId { get; set; }

    public int? Duration { get; set; }
  

    public bool Required { get; set; }

    public virtual SkillCategory? SkillCategory { get; set; }

    public virtual SkillLevel? SkillLevel { get; set; }

    public virtual SkillSubcategory? SkillSubcategory { get; set; }

    public virtual Vacancy Vacancy { get; set; } = null!;
}
