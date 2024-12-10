namespace JobPlatform.DTO.Skill_Level
{
    public class SkillLevelViewDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? SkillSort { get; set; }
        public string? SortName {  get; set; }   

        public int? SkillCategoryId { get; set; }
        public string? CategoryName { get; set; }

        public long? SkillSubcategoryId { get; set; }
        public string? SubcategoryName { get; set; }

        public int? Grade { get; set; } = 0;
    }
}
