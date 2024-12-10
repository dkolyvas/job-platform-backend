using JobPlatform.Data;
using JobPlatform.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class SkillSubcategoryRepository : BaseRepository<SkillSubcategory>
    {
        public SkillSubcategoryRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<IEnumerable<SkillSubcategory>> FindSubcategories(int catId)
        {
            return await _db.SkillSubcategories.Include(s => s.SkillCategory).Where(ssc => ssc.SkillCategory.Id == catId).ToListAsync();
        }

        public override async Task<SkillSubcategory?> FindById(long id)
        {
            return await _db.SkillSubcategories.Include(s => s.SkillCategory).Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> MergeSubcategories(long mergedSubcategoryId, long remainingSubacateogoryId)
        {
            var remainingSubCat = await _db.SkillSubcategories.FindAsync(remainingSubacateogoryId);
            if (remainingSubCat is null) throw new EntityNotFoundException("skill subcategory");
            try
            {
                _db.Database.BeginTransaction();

                string queryAppend = $"SET SKILL_SUBCATEGORY_ID = {remainingSubacateogoryId} WHERE SKILL_SUBCATEGORY_ID = {mergedSubcategoryId}";

                string query = $"UPDATE Vacancy_Skills {queryAppend}";
                var result = await _db.Database.ExecuteSqlRawAsync(query);
                query = $"UPDATE Applicant_Skills {queryAppend}";
                result += await _db.Database.ExecuteSqlRawAsync(query);
                query = $"UPDATE Skill_Levels {queryAppend}";
                result += await _db.Database.ExecuteSqlRawAsync(query);
                query = $"DELETE FROM Skill_Subcategories WHERE ID = {mergedSubcategoryId}";
                result += await _db.Database.ExecuteSqlRawAsync(query);
                _db.Database.CommitTransaction();
                return result;
            }
            catch (Exception )
            {
                _db.Database.RollbackTransaction();
                throw new UnableToSaveDataException();
            }
        }
            

        public async Task<IEnumerable<SkillSubcategory>> FindUnchecked()
        {
            return await _db.SkillSubcategories.Where(s => s.Checked == false).ToListAsync();
        }

        public async Task<SkillSubcategory?> UpdateSubcategory(SkillSubcategory subcategory)
        {
            var dbValue = await _db.SkillSubcategories.FindAsync(subcategory.Id);
            if (dbValue is not null)
            {
                subcategory.Checked = dbValue.Checked;
                _table.Entry(dbValue).CurrentValues.SetValues(subcategory);
                _table.Entry(dbValue).State = EntityState.Modified;
            }
            return dbValue;
        }

        public async Task<SkillSubcategory?> MarkAsChecked(long id)
        {
            var subcategory = await _db.SkillSubcategories.FindAsync(id);
            if (subcategory is not null)
            {
                subcategory.Checked = true;
                _table.Entry(subcategory).State = EntityState.Modified;
            }
            return subcategory;
        }
    }
    
}
