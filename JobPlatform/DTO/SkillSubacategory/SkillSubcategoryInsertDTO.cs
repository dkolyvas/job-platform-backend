using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.SkillSubacategory
{
    public class SkillSubcategoryInsertDTO
    {

        [Required(ErrorMessage = "You must specify a name for the subcategory")]
        [StringLength(45, ErrorMessage = "The name cannot exceed 45 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "You must specify a skill category")]
        public int SkillCategoryId { get; set; }

        
    }
}
