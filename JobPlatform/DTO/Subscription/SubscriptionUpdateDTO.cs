using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Subscription
{
    public class SubscriptionUpdateDTO
    {
        [Required(ErrorMessage = "The id of the subscription is a required field")]
        public long Id { get; set; }

        [Required(ErrorMessage = "The business id is a required field")]
        public long BusinessId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date, ErrorMessage = "You must provide start date in a valid date format")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date, ErrorMessage = "You must provide end date in a valid data format")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "You must specify the postings allowance for the subscription")]
        public int Allowance { get; set; }
    }
}
