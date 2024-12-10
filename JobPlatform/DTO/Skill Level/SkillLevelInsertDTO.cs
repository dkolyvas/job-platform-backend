using System.ComponentModel.DataAnnotations;
namespace JobPlatform.DTO.Skill_Level
{
    public class SkillLevelInsertDTO
    {
        [Required(ErrorMessage ="You must specify a name")]
        [StringLength(45, ErrorMessage = "The name cannot exceed 45 characters")]
        public string? Name { get; set; }

        [AllowedValues([null, 0, 1, 2], ErrorMessage = "The skill sort must have a value from 0-2 if defined")]
        public int? SkillSort { get; set; }

        public int? SkillCategoryId { get; set; }

        public long? SkillSubcategoryId { get; set; }

        public int? Grade { get; set; } = 0;
    }
}
