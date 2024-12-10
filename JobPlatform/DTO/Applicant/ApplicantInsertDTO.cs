using JobPlatform.DTO.Applicant.ApplicantMerits;
using JobPlatform.DTO.Applicant.ApplicantSkills;
using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Applicant
{
    public class ApplicantInsertDTO
    {
        //     public long Id { get; set; }

        [Required(ErrorMessage = "You must specify the first name")]
        [RegularExpression(@"^[\p{L}\s]*$", ErrorMessage = "The first name can contain only letters")]
        [StringLength(50, ErrorMessage = "The first name cannot exceed 50 characters")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "You must specify the last name")]
        [RegularExpression(@"^[\p{L}\s]*$", ErrorMessage = "The last name can contain only letters")]
        [StringLength(50, ErrorMessage = "The last name cannot exceed 50 characters")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "You must specify an email")]
        [EmailAddress(ErrorMessage = "You must provide a valid email address")]
        [StringLength(70, ErrorMessage = "Email address cannont exceed 70 characters")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "The phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        public long? UserId { get; set; }


        public List<ApplicantSkillInsertDTO> Skills { get; set; } = new();

        public List<ApplicantMeritInsertDTO> Merits { get; set; } = new();
    }
}
