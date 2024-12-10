using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.User
{
    public class UserRestorationDTO
    {
      [Required(ErrorMessage = "You must submit a username")]
      public string UserName { get; set; }

      public  string? RestorationCode { get; set; }

      [RegularExpression(@"(?=.*[A-Z)(?=.*[a-z])(?=.*\d)(?=.*\W)^.{8,}$",
        ErrorMessage = "The password must contain at least one small and one capital letter, a digit and a symbol")]
       public string? NewPassword { get; set; }
       public string? ConfirmPassword { get; set; }

    }
}
