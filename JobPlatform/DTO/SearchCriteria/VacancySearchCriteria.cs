namespace JobPlatform.DTO.SearchCriteria
{
    public class VacancySearchCriteria
    {
        public List<int> Regions { get; set; }= new List<int>();
        public int? JobCategory { get; set; }
        public int? JobSubCategory { get; set; }

        public List<VacancySkillSearchCriteria> SkillSearchCriteria { get; set; } = new List<VacancySkillSearchCriteria>();

    }




    public class VacancySkillSearchCriteria
    {
        public int? CategoryId { get; set; }
        public long? SubcategoryId { get; set; }
        public int? SkillLevelGrade { get; set; } = 1;

        public int? SkillDuration { get; set; } = 0;
    }
}
