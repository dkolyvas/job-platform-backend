namespace JobPlatform.DTO.Subscription
{
    public class SubscriptionViewDTO
    {
        public long Id { get; set; }

        public long? BusinessId { get; set; }

        public string? BusinessName { get; set; }   

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int? Allowance { get; set; }

        public int? VacancyPostsCount { get; set; }

        
    }
}
