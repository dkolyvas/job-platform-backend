namespace JobPlatform.DTO.SkillCategory
{
    public class SkillCategoryViewDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int Sort { get; set; }

        public string? SortName { get; set; }


        public bool? Checked { get; set; }
    }
}
