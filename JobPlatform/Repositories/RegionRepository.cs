using JobPlatform.Data;
using JobPlatform.Exceptions;

namespace JobPlatform.Repositories
{
    public class RegionRepository : BaseRepository<Region>
    {
        public RegionRepository(JobplatformContext db) : base(db)
        {
        }

        public  async Task<bool> Delete(int id)
        {
            var region = await _db.Regions.FindAsync(id);
            if(region != null)
            {
                if (region.Vacancies.Count > 0) throw new UnableToDeleteException();
                _db.Regions.Remove(region);
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<Region?> UpdateOne(Region updatedData)
        {
            var dbValue = await _db.Regions.FindAsync(updatedData.Id);
            if (dbValue != null)
            {
                _table.Entry(dbValue).CurrentValues.SetValues(updatedData);
                _table.Entry(dbValue).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            return dbValue;
        }

        public async Task<Region?> FindById(int id)
        {
            return await _db.Regions.FindAsync(id);
        }

    }
}
