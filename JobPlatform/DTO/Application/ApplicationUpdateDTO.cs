using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Application
{
    public class ApplicationUpdateDTO
    {
        [Required(ErrorMessage = "You must provide the id of the application")]
        public long Id { get; set; }


        
        public long? ApplicantId { get; set; }

        public string? ApplicationText { get; set; }


        
    }
}
