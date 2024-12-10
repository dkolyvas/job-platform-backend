namespace JobPlatform.DTO.Vacancy.VacancySkills
{
    public class VacancySkillViewDTO
    {
        public long Id { get; set; }

        public long VacancyId { get; set; }

        public int? SkillSort { get; set; }

        public int? SkillCategoryId { get; set; }

        public string? SkillCategoryName { get; set; }

        public long? SkillSubcategoryId { get; set; }
        public string? SkillSubcategoryName { get; set; }

        public int? SkillLevelId { get; set; }
        public string? SkillLevelName { get; set; }

        

        public int? Duration { get; set; }


        public bool Required { get; set; }
    }
}
