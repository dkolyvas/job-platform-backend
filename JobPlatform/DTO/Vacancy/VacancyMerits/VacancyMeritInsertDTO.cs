using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Vacancy.VacancyMerits
{
    public class VacancyMeritInsertDTO
    {
        // public long Id { get; set; }

        //   public long? VacancyId { get; set; }

        [Required(ErrorMessage = "You must specify all desired merits for the vacancy")]
        [StringLength( 65, ErrorMessage = "Merit descriptions cannot exceed 65 characters")]
        public string? Name { get; set; }
    }
}
