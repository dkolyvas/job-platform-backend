using JobPlatform.Data;
using JobPlatform.Exceptions;
using JobPlatform.Util;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class ApplicantRepository : BaseRepository<Applicant>
    {
        public ApplicantRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<IEnumerable<Applicant>> FindApplicants(string? firstname, string? lastname, string? email)
        {
            var query = _db.Applicants.AsQueryable();
            if (firstname is not null) query = query.Where(a => a.Firstname.StartsWith(firstname));
            if(lastname is not null) query = query.Where(a => a.Lastname.StartsWith(lastname));
            if(email is not null) query = query.Where(a => a.Email== email);

            return await query.ToListAsync();
        }


        public async Task<Applicant?> FindApplicantByUserId(long? userId)
        {
            return await _db.Applicants.Where(a => a.UserId == userId).FirstOrDefaultAsync();
        }

        public override async Task<bool> Delete(long id)
        {
            var applicant = await _db.Applicants.FindAsync(id);
            
            if (applicant != null)
            {
               _db.ApplicantMerits.RemoveRange(applicant.ApplicantMerits);
               _db.ApplicantSkills.RemoveRange(applicant.ApplicantSkills);
               _db.Applications.RemoveRange(applicant.Applications);
                var user = applicant.User;
                if(user is not null) _db.Users.Remove(user);
                _db.Applicants.Remove(applicant);
                return true;
            }
            else { return false; }
        }

        public override async Task<Applicant?> UpdateOne(Applicant entity, long id)
        {
            var dbValue = await _db.Applicants.Include(a => a.ApplicantSkills).Include(a => a.ApplicantMerits)
                .Where(a => a.Id == id).FirstOrDefaultAsync();
            if (dbValue is not null)
            {
                string? imgStr = dbValue.Cv;
                long? userId = dbValue.UserId;
                _table.Entry(dbValue).CurrentValues.SetValues(entity);
                dbValue.Cv = imgStr;
                dbValue.UserId = userId;
                _table.Entry(dbValue).State = EntityState.Modified;
            }
            return dbValue;
        }

        public async Task<Applicant?> UpdateCv(string cvString, long userId)
        {
            var dbValue = await _db.Applicants.Where(a => a.UserId == userId).FirstOrDefaultAsync();
            if (dbValue is not null)
            {
                if (dbValue.Cv != null)
                {
                    FileProcessor.DeletePdf(dbValue.Cv);
                }
                dbValue.Cv = cvString;
                _table.Entry(dbValue).State |= EntityState.Modified;
            }
            return dbValue;
        }

        public async Task<Applicant?> FindApplicantForBusiness(long applicantId, long businessId)
        {
            var applicant = await _db.Applicants.Include(a => a.Applications)
                .ThenInclude(a =>a.Vacancy)
                .Where(a => a.Id == applicantId).FirstOrDefaultAsync();
            
            if(applicant is not null && !applicant.Applications.Any(a => a.Vacancy.BusinessId == businessId))
            {
                throw new AccessNotAllowedException();
            }
            return applicant;
            
            
        }

    }
}
