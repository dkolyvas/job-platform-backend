using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Application
{
    public class ApplicationInsertDTO
    {
        [Required(ErrorMessage = "You must specify the vacancy the application is made for")]
        public long VacancyId { get; set; }

      
        public long? ApplicantId { get; set; }

        public string? ApplicationText { get; set; }
       



    }
}
