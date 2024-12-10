using JobPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class SubscriptionRepository : BaseRepository<Subscription>
    {
        public SubscriptionRepository(JobplatformContext db) : base(db)
        {
        }

        public override async Task<Subscription?> FindById(long id)
        {
            return await _db.Subscriptions.Include(b => b.Business).Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Subscription?> FindMemberActiveSubscription(long? businessId)
        {
            return await _db.Subscriptions.Where(s =>s.BusinessId == businessId && s.StartDate != null && s.EndDate != null &&
             s.StartDate <= DateOnly.FromDateTime(DateTime.Now) && s.EndDate >= DateOnly.FromDateTime(DateTime.Now)).FirstOrDefaultAsync();
            
        }

        public async Task<IEnumerable<Subscription>> FindCurrentAndFutureSubscriptions(long? businessId)
        {
            return await _db.Subscriptions.Where( s => s.BusinessId== businessId && s.EndDate != null
            && s.EndDate >= DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> FindMemberSubscriptionHistory(long? businessId)
        {
            return await _db.Subscriptions.Include(s =>s.Business).Where(s => s.BusinessId == businessId).ToListAsync();
        }

        public  Subscription StopSubscription(Subscription subscription)
        {
            subscription.EndDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);
            _table.Entry(subscription).State = EntityState.Modified;
            return subscription;
        }
        
    }
}
