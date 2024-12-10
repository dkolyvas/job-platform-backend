using JobPlatform.Data;
using JobPlatform.Exceptions;
using JobPlatform.Util;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class BusinessRepository : BaseRepository<Business>
    {
        public BusinessRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<IEnumerable<Business>> FindBusinessByName(string? name, string? email)
        {
            var query = _db.Businesses.AsQueryable();
            if (name is not null) query = query.Where(b =>b.Name !=null && b.Name.StartsWith(name)).Include(b => b.User);
            if (email is not null) query = query.Where(b => b.Email == email).Include(b => b.User);
            return await query.ToListAsync();
        }

        public override async Task<IEnumerable<Business>> FindAll()
        {
            return await _db.Businesses.Include(b => b.User).ToListAsync();
        }

        public async Task<Business?> FindBusinessByUser(long userId)
        {
            return await _db.Businesses.Include(b => b.User).Where(b => b.UserId == userId).FirstOrDefaultAsync();
        }

       

        public override async Task<Business?> FindById(long id)
        {
            return await _db.Businesses.Include(b => b.User).Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public override async Task<bool> Delete(long id)
        {
            var business = await _db.Businesses.FindAsync(id);
            if (business == null) return false;
            if(business.Vacancies.Count > 0 || business.Subscriptions.Count >0)
            {
                throw new UnableToDeleteException();
            }
            _db.Businesses.Remove(business);
            return true;
        }

        public override async Task<Business?> UpdateOne(Business entity, long id)
        {
            var dbValue = await _db.Businesses.FindAsync(id);
            
            if (dbValue is not null)
            {
                string? imgStr = dbValue.Image;
                long? userId = dbValue.UserId;
                _table.Entry(dbValue).CurrentValues.SetValues(entity);
                dbValue.Image = imgStr;
                dbValue.UserId = userId;
                _table.Entry(dbValue).State = EntityState.Modified;
            }
            return dbValue;
        }

        public async Task<Business?> UpdateImage(string imgStr, long userId)
        {
            var dbValue = await _db.Businesses.Where(b => b.UserId == userId).FirstOrDefaultAsync();
            if(dbValue is not null)
            {
                if(dbValue.Image != null)
                {
                    FileProcessor.DeleteImage(dbValue.Image);
                }
                dbValue.Image = imgStr;
                _table.Entry(dbValue).State |= EntityState.Modified;
            }
            return dbValue;
        }
    }
}
