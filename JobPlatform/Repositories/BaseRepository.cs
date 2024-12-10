
using JobPlatform.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public JobplatformContext _db;
        protected DbSet<T> _table;

        public BaseRepository(JobplatformContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }

        public async Task<T> AddOne(T entity)
        {
            await _table.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> AddRange(IEnumerable<T> entities)
        {
            await _table.AddRangeAsync(entities);
            return entities;
        }

        public virtual async Task<bool> Delete(long id)
        {
            T? entity = await _table.FindAsync(id);
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

        public virtual async Task<IEnumerable<T>> FindAll()
        {
           return  await _table.ToListAsync();
        }

        public virtual async Task<T?> FindById(long id)
        {
            return await _table.FindAsync(id);
        }

        public virtual async Task<T?> UpdateOne(T entity, long id)
        {
            var val = await _table.FindAsync(id);
            if (val is not null)
            {
                _table.Entry(val).CurrentValues.SetValues(entity);
                _table.Entry(val).State = EntityState.Modified;
            }
            return val;
        }
    }
}
