using JobPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class VacancySkillRepository : BaseRepository<VacancySkill>
    {
        public VacancySkillRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<List<VacancySkill>> AddVacancySkills(Vacancy vacancy, List<VacancySkill> skills)
        {
            foreach(var skill in skills)
            {
                skill.Vacancy = vacancy;
            }
            await _db.AddRangeAsync(skills);
            
            return await GetVacancySkills(vacancy.Id);
        }

        public async Task<List<VacancySkill>> UpdateVacancySkills(Vacancy vacancy, List<VacancySkill> skills)
        {
            var deleteList = await _db.VacancySkills.Where(s => s.VacancyId == vacancy.Id).ToListAsync();
            _db.RemoveRange(deleteList);
            return await AddVacancySkills(vacancy, skills);
        }

        public async Task<List<VacancySkill>> GetVacancySkills(long? vacancyId)
        {
            return await _db.VacancySkills.Include(s => s.SkillCategory)
                .Include(s => s.SkillSubcategory).ThenInclude( s => s.SkillCategory)
                .Include(s => s.SkillLevel)
                .Where(s => s.VacancyId == vacancyId).ToListAsync();
        }
    }
}
