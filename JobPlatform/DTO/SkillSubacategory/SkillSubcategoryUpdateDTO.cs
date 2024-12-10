using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.SkillSubacategory
{
    public class SkillSubcategoryUpdateDTO
    {
        [Required(ErrorMessage = "You must specify the subcategory id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "You must specify a name for the subcategory")]
        [StringLength(45, ErrorMessage = "The name cannot exceed 45 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "You must specify a skill category")]
        public int SkillCategoryId { get; set; }

       // public bool? Checked { get; set; }
    }
}
