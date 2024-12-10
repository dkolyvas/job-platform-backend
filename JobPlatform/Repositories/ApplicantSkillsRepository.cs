using JobPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class ApplicantSkillsRepository : BaseRepository<ApplicantSkill>
    {
        public ApplicantSkillsRepository(JobplatformContext db) : base(db)
        {
        }

        public void DeleteApplicantSkills(Applicant applicant)
        {
            var deleteList = applicant.ApplicantSkills;
            if(deleteList.Count >0) _db.RemoveRange(deleteList);

        }

        public async Task<bool> DeleteApplicantSkills(long applicantId)
        {
            var deleteList = await _db.ApplicantSkills.Where(s => s.ApplicantId == applicantId).ToListAsync();
            if (deleteList.Count > 0)  _db.RemoveRange(deleteList);
            return true;
        }

        public async Task<List<ApplicantSkill>> UpdateApplicantSkills(Applicant applicant, List<ApplicantSkill> applicantSkills) {
            DeleteApplicantSkills(applicant);
            foreach(var skill in applicantSkills)
            {
                skill.Applicant = applicant;
            }
            if(applicantSkills.Count >0) await _db.AddRangeAsync(applicantSkills);
            return applicantSkills;
        }

        public async Task<List<ApplicantSkill>> AddApplicantSkills(Applicant applicant, List<ApplicantSkill> applicantSkills)
        {
            foreach (var skill in applicantSkills)
            {
                skill.Applicant = applicant;
            }
            if (applicantSkills.Count > 0) await _db.AddRangeAsync(applicantSkills);
            return applicantSkills;
        }

        public async Task<List<ApplicantSkill>> GetApplicantSkills(long? applicantId)
        {
            return await _db.ApplicantSkills.Include(a => a.SkillSubcategory).ThenInclude(s => s.SkillCategory)
                .Include(a => a.SkillLevel).Where(a => a.ApplicantId == applicantId).ToListAsync();
        }
    }
}
