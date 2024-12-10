using JobPlatform.Data;
using JobPlatform.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JobPlatform.Repositories
{
    public class SkillCategoryRepository : BaseRepository<SkillCategory>
    {
        public SkillCategoryRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<IEnumerable<SkillCategory>> FindCategoriesBySort(int sortId)
        {
            return await _db.SkillCategories.Where(sc => sc.Sort == sortId).ToListAsync();

        }
        public  async Task<SkillCategory?> FindById(int id)
        {
            return await _table.FindAsync(id);
        }
        public  async Task<SkillCategory?> UpdateOne(SkillCategory category)
        {
            var val = await _table.FindAsync(category.Id);
            if (val is not null)
            {
                _table.Entry(val).CurrentValues.SetValues(category);
                _table.Entry(val).State = EntityState.Modified;
            }
            return val;
        }

        public async Task<int> MergeCategories(int mergedCategoryId, int remainingCategoryId) {
            var remainingCategory = await _db.SkillCategories.FindAsync(remainingCategoryId);
            if (remainingCategory is null) throw new EntityNotFoundException("skill category");
            try
            {
                
                _db.Database.BeginTransaction();

                string queryAppend = $"SET SKILL_CATEGORY_ID = {remainingCategoryId} WHERE SKILL_CATEGORY_ID = {mergedCategoryId}";

                string query = $"UPDATE Vacancy_Skills {queryAppend}";
                var result = await _db.Database.ExecuteSqlRawAsync(query);
                query = $"UPDATE Skill_Subcategories {queryAppend}";
                result += await _db.Database.ExecuteSqlRawAsync(query);
                query = $"UPDATE Skill_Levels {queryAppend}";
                result += await _db.Database.ExecuteSqlRawAsync(query);
                query = $"DELETE FROM Skill_Categories WHERE ID = {mergedCategoryId}";
                result += await _db.Database.ExecuteSqlRawAsync(query);
                _db.Database.CommitTransaction();

                return result;

            }
            catch (Exception)
            {
                _db.Database.RollbackTransaction();
                throw new UnableToSaveDataException();
            }
        }

      

        
    }
}
