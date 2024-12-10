using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Vacancy.VacancySkills
{
    public class VacancySkillInsertDTO
    {

        //  public long VacancyId { get; set; }

        [Required(ErrorMessage = "You must specify the sort of all skills")]
        public int SkillSort { get; set; }

        public int? SkillCategoryId { get; set; }

        public long? SkillSubcategoryId { get; set; }

        public int? SkillLevelId { get; set; }

        public int? Duration { get; set; }

        [Required(ErrorMessage = "You must specify for all skills whethter they are required")]
        public bool Required { get; set; } = false;
    }
}
