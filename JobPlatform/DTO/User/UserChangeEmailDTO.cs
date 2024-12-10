using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.User
{
    public class UserChangeEmailDTO
    {
        [Required(ErrorMessage ="id is required")]
        public long Id { get; set; }

        [Required(ErrorMessage ="You must specify an email")]
        [EmailAddress(ErrorMessage ="You must provide a valid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage ="Password is required")]
        public string? Password { get; set; }
    }
}
