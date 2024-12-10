using JobPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class ApplicantMeritsRepository : BaseRepository<ApplicantMerit>
    {
        public ApplicantMeritsRepository(JobplatformContext db) : base(db)
        {
        }

        public void DeleteApplicatMerits(Applicant applicant)
        {
            var deleteList = applicant.ApplicantMerits;
            if (deleteList.Count > 0) _db.RemoveRange(deleteList);
        }

        public async Task<bool> DeleteApplicantMerits(long applicantId)
        {

            var deleteList = await _db.ApplicantMerits.Where(m => m.ApplicantId == applicantId).ToListAsync();
            if (deleteList.Count > 0) _db.RemoveRange(deleteList);
            return true;
        }

        public async Task<List<ApplicantMerit>> AddApplicantMerits(Applicant applicant, List<ApplicantMerit> merits)
        {
            foreach(var merit in merits)
            {
                merit.Applicant = applicant;
            }
            if(merits.Count > 0) await _db.AddRangeAsync(merits);
            return merits;
        }

        public async Task<List<ApplicantMerit>> UpdateApplicantMerits(Applicant applicant, List<ApplicantMerit> merits)
        {
            DeleteApplicatMerits(applicant);
            
            return await AddApplicantMerits(applicant, merits);
        }

        public async Task<List<ApplicantMerit>> GetApplicantMerits(long applicantId)
        {
            return await _db.ApplicantMerits.Where(m => m.ApplicantId == applicantId).ToListAsync();
        }
    }
}
