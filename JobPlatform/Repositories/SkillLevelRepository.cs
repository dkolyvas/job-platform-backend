using JobPlatform.Data;
using JobPlatform.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class SkillLevelRepository : BaseRepository<SkillLevel>
    {
        public SkillLevelRepository(JobplatformContext db) : base(db)
        {
        }

        /**
         * The functions searches at first for skill levels associated with skill subcategory if specified
         * If there are no results it continues for skill levels associated with the (if specified) skill
         * category and if there are still no results it continuess the search for the specified skill
         * sort. 
         * Note that skill levels associated with skill subcategories take precedence over those associated with 
         * skill categories and these take precedence over those associated with skill sorts.
         * If skill levels are specified for a lower hierarchy level it is as if those on a higher 
         * level did not exist since the function won't search for them.
         */

        public async Task<IEnumerable<SkillLevel>> FindSkillLevels(int? sort, int? catId, long? subId)
        {
            List<SkillLevel> results = new();
           /* if(sort != null)
            {
                results = await _db.SkillLevels.Where(l => l.SkillSort == sort).ToListAsync();
            }
            if(catId != null  && results.Count == 0)           
            {
                results = await _db.SkillLevels.Where(l => l.SkillCategoryId== catId).ToListAsync();
            }
            if(subId != null && results.Count == 0)
            {
                results = await _db.SkillLevels.Where(l => l.SkillSubcategoryId== subId).ToListAsync();
            }*/

            if(subId != null)
            {
                results = await _db.SkillLevels.Where(l => l.SkillSubcategoryId == subId).ToListAsync();
            }
            if (catId != null && results.Count == 0)
            {
                results = await _db.SkillLevels.Where(l => l.SkillCategoryId == catId).ToListAsync();
            }
            if (sort != null && results.Count == 0)
            {
                results = await _db.SkillLevels.Where(l => l.SkillSort == sort).ToListAsync();
            }


            return results;
        }

        public  async Task<SkillLevel?> FindById(int id)
        {
            return await _db.SkillLevels.Include(l => l.SkillCategory).Include(l => l.SkillSubcategory).Where(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task<SkillLevel?> UpdateOne(SkillLevel skillLevel)
        {
            var val = await _table.FindAsync(skillLevel.Id);
            if (val is not null)
            {
                _table.Entry(val).CurrentValues.SetValues(skillLevel);
                _table.Entry(val).State = EntityState.Modified;
            }
            return val;
        }

        public async Task<IEnumerable<SkillLevel>> FindSkillLevelsForSubcategory(long subId)
        {
            List<SkillLevel> results = new();
            SkillSubcategory? subcategory = await _db.SkillSubcategories.Include(c => c.SkillCategory).Where(c => c.Id == subId).FirstOrDefaultAsync();
            if(subcategory != null)
            {
                int? catId = subcategory.SkillCategoryId;
                int? sort = subcategory.SkillCategory.Sort;
                results = await _db.SkillLevels.Where(l => l.SkillSubcategoryId == subId).ToListAsync();
                if (catId != null && results.Count == 0)
                {
                    results = await _db.SkillLevels.Where(l => l.SkillCategoryId == catId).ToListAsync();
                }
                if (sort != null && results.Count == 0)
                {
                    results = await _db.SkillLevels.Where(l => l.SkillSort == sort).ToListAsync();
                }
               
               

            }
            return results;
        }

        public async Task<IEnumerable<SkillLevel>> FindSkillLevelsForCategory(int catId)
        {
            List<SkillLevel> results = new();
            SkillCategory? category = await _db.SkillCategories.FindAsync(catId);
            if(category != null)
            {
                int? sort = category.Sort;
                results = await _db.SkillLevels.Where(l => l.SkillCategoryId == catId).ToListAsync();
                if (sort != null && results.Count == 0)
                {
                    results = await _db.SkillLevels.Where(l => l.SkillSort == sort).ToListAsync();
                }
                
            }
            return results;
        }

        public async Task<IEnumerable<SkillLevel>> FindSkillLevelsForSort(int sortId)
        {
            List<SkillLevel> results = new();
            results = await _db.SkillLevels.Where(l => l.SkillSort == sortId).ToListAsync();
            return results;
        }

        public  async Task<bool> Delete(int id)
        {
            var level = await _db.SkillLevels.FindAsync(id);
            if(level is null) return false;
            if(level.ApplicantSkills.Count > 0 || level.VacancySkills.Count > 0) 
            {
                throw new UnableToDeleteException();
            }
            _db.SkillLevels.Remove(level);
            return true;
        }
    }
}
