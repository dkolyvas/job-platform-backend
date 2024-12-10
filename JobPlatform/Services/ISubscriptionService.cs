using JobPlatform.DTO.Subscription;

namespace JobPlatform.Services
{
    public interface ISubscriptionService
    {
        public Task<SubscriptionViewDTO> AddOrRenewSubscription(SubscriptionInsertDTO insertDTO);
        public Task<SubscriptionViewDTO> CancelSubscription(long subscriptionId);
        public Task<SubscriptionViewDTO> CancelSubscription(long subscriptionId, long businessId);
        public Task<bool> DeleteSubscription(long subscriptionId);
        public Task<IEnumerable<SubscriptionViewDTO>> GetBusinessSubscriptions(long businessId);

        public Task<SubscriptionViewDTO> GetSubscription(long subscriptionId);

        public Task<SubscriptionViewDTO> Update(SubscriptionUpdateDTO updateDTO);
    }
}
