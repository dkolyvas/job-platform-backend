using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.User
{
    public class UserChangePasswordDTO
    {
        [Required(ErrorMessage ="Id is required")]
        public long Id { get; set; }
        [Required(ErrorMessage ="Password is requird")]
        public string? Password { get; set; }
        [RegularExpression(@"(?=.*[A-Z)(?=.*[a-z])(?=.*\d)(?=.*\W)^.{8,}$", 
           ErrorMessage = "The password nust contain at least a small, a capital letter, a digit and a symbol")]
        
        public string? NewPassword {  get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
