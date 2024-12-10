using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.SkillCategory
{
    public class SkillCategoryInsertDTO
    {
        
        [Required(ErrorMessage = "You must specify a category name")]
        [StringLength(45, ErrorMessage = "The category name cannot exceed 45 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "You must specify a skill sort")]
        [AllowedValues([0,1,2], ErrorMessage = "Skill Sort must have a value 0-2" )]
        public int Sort { get; set; }

        public bool? Checked { get; set; } = true;
    }
}
