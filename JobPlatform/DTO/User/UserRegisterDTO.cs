using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.User
{
    public class UserRegisterDTO
    {
        [StringLength(20,MinimumLength =2,  ErrorMessage ="The username must have at least 2 characters and not exceed 20")]
        [Required(ErrorMessage ="Username is required")]
        public string? Username { get; set; }
        [RegularExpression(@"(?=.*[A-Z)(?=.*[a-z])(?=.*\d)(?=.*\W)^.{8,}$", 
            ErrorMessage ="The password must contain at least one small and one capital letter, a digit and a symbol")]

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage = "You must provide a valid email address")]

        public string? Email { get; set; }
        

        public int? RoleId { get; set; }

    }
}
