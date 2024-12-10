using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Business
{
    public class BusinessUpdateDTO
    {
        [Required(ErrorMessage = "You must specify an id")]
        public long Id { get; set; }
        [Required(ErrorMessage = "Business Name is required")]
        [StringLength(70, ErrorMessage = "The Business Name must not exceed 70 characters")]

        public string? Name { get; set; }
        [Required(ErrorMessage = "You must specify an address")]
        [StringLength(200, ErrorMessage = "The address must not exceed 200 characters")]

        public string? Address { get; set; }

        [Required(ErrorMessage = "You must specify a phon number")]
        [StringLength(20, ErrorMessage = "The phone number must not exceed 20 characters")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "You must specify an email")]
        [EmailAddress(ErrorMessage = "You must submit a valid email address")]
        public string? Email { get; set; }

        [StringLength(70, ErrorMessage = "The website address cannot exceed 70 characters")]
        public string? Website { get; set; }

        
        
    }
}
