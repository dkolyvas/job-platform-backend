using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.SubscriptionType
{
    public class SubscriptionTypeUpdateDTO
    {
        [Required(ErrorMessage = "You must specify the id")]
        public int Id { get; set; }

        [StringLength(45, ErrorMessage = "The name cannot exceed 45 characters")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "You must specify the price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "You must specify the duration")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "You must specify the allowance")]
        public int Allowance { get; set; }
       
        }
}
