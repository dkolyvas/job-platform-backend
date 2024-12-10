using System.ComponentModel.DataAnnotations;

namespace JobPlatform.DTO.Subscription
{
    public class SubscriptionInsertDTO
    {
        [Required(ErrorMessage = "You must specify the business for the subscritpion")]
        public long BusinessId { get; set; }
        [Required(ErrorMessage = "You must specify the subscription type")]
        public int SubscriptionTypeId {  get; set; }
        public bool ReplaceExisting { get; set; }=false;
    }
}
