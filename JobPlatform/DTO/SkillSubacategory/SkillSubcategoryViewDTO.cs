namespace JobPlatform.DTO.SkillSubacategory
{
    public class SkillSubcategoryViewDTO
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public int SkillCategoryId { get; set; }

        public string? CategoryName { get; set; }
        public string? SortName { get; set; }
        
    }
}
