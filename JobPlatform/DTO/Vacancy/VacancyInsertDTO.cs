using JobPlatform.Data;
using JobPlatform.DTO.Vacancy.VacancyMerits;
using JobPlatform.DTO.Vacancy.VacancySkills;
using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Vacancy
{
    public class VacancyInsertDTO
    {

        [Required(ErrorMessage = "You must specify a business")]
        public long BusinessId { get; set; } 

        [StringLength(200, ErrorMessage = "The address cannot be longer than 200 characters")]
        public string? Address { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "You must provide a title for the vacancy")]
        [StringLength(45, ErrorMessage = "The vacancy title cannot exceed 45 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "You must provide the object of the vacancy")]
        public long SkillSubcategoryId { get; set; }

        [Required(ErrorMessage = "You must specify the region where the job is located")]
        public int RegionId { get; set; }

        

        public List<VacancySkillInsertDTO> Skills { get; set; } = new();

        public List<VacancyMeritInsertDTO> Merits { get; set; } = new();


        
    }
}
