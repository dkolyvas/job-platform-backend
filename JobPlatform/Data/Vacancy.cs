using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class Vacancy
{
    public long Id { get; set; }

    public long? BusinessId { get; set; }

    public bool? Active { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string Title { get; set; } = null!;

    public long SkillSubcategoryId { get; set; }

    public int RegionId { get; set; }

    public DateOnly? PublicationDate { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Business? Business { get; set; }

    public virtual Region Region { get; set; } = null!;

    public virtual SkillSubcategory SkillSubcategory { get; set; } = null!;

    public virtual ICollection<VacancyMerit> VacancyMerits { get; set; } = new List<VacancyMerit>();

    public virtual ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
}
