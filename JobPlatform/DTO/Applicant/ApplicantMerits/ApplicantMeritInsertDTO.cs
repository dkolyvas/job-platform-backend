using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace JobPlatform.DTO.Applicant.ApplicantMerits
{
    public class ApplicantMeritInsertDTO
    {

       // [Required(ErrorMessage = "You must specify an applicant for all merits")]
       // public long? ApplicantId { get; set; }

        [Required(ErrorMessage = "You must provide a description for all merits")]
        [StringLength(50, ErrorMessage = "Merit descriptions cannont exceed 50 characters")]
        public string? Name { get; set; }

       
    }
}
