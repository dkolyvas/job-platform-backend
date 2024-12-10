using JobPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class SubscriptionTypeRepository : BaseRepository<SubscriptionType>
    {
        public SubscriptionTypeRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<SubscriptionType?> UpdateOne(SubscriptionType updatedData)
        {
            SubscriptionType? dbData = await _db.SubscriptionTypes.FindAsync(updatedData.Id);
            if(dbData is not null)
            {
                _table.Entry(dbData).CurrentValues.SetValues(updatedData);
                _table.Entry(dbData).State = EntityState.Modified;
            }
            return dbData;
        }

        public virtual async Task<bool> Delete(int id)
        {
            SubscriptionType? entity = await _table.FindAsync(id);
            if (entity is not null)
            {
                _table.Remove(entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual async Task<SubscriptionType?> FindById(int id)
        {
            return await _table.FindAsync(id);
        }


    }
}
