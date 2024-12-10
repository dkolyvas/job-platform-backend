using JobPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class VacancyMeritRepository : BaseRepository<VacancyMerit>
    {
        public VacancyMeritRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<List<VacancyMerit>> AddVacancyMerits(Vacancy vacancy, List<VacancyMerit> merits)
        {
            foreach(var merit in merits)
            {
                merit.Vacancy = vacancy;
            }
             await _db.AddRangeAsync(merits);
            return merits;
        }

        public async Task<List<VacancyMerit>> UpdateVacancyMerits(Vacancy vacancy, List<VacancyMerit> merits)
        {
            var deleteList = await GetVacancyMerits(vacancy.Id);
            _db.RemoveRange(deleteList);

            return await AddVacancyMerits(vacancy, merits);
        }

        public async Task<List<VacancyMerit>> GetVacancyMerits(long vacancyId)
        {
            return await _db.VacancyMerits.Where(m => m.VacancyId == vacancyId).ToListAsync();
        }
    }
}
