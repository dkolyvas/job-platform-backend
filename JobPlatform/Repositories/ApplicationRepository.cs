using JobPlatform.Data;
using JobPlatform.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Repositories
{
    public class ApplicationRepository : BaseRepository<Application>
    {
        public ApplicationRepository(JobplatformContext db) : base(db)
        {
        }

        public async Task<IEnumerable<Application>> GetVacancyApplications(long vacancyId)
        {
            return await _db.Applications.Include(a =>a.Applicant).Include(a=> a.Vacancy).ThenInclude(v => v.Business)                
                .Where(a => a.VacancyId == vacancyId).ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetVacancyUncheckedApplications(long vacancyId)
        {
            return await _db.Applications.Include(a => a.Applicant).Include(a => a.Vacancy).ThenInclude(v => v.Business)
                .Where(a => a.VacancyId == vacancyId && a.Checked==false).ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetVacacnyApprovedApplications(long vacancyId)
        {
            return await _db.Applications.Include(a => a.Applicant).Include(a => a.Vacancy).ThenInclude(v => v.Business)
                .Where(a => a.VacancyId == vacancyId && a.Approved == true).ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetApplicantApplications(long applicantId)
        {
            return await _db.Applications.Include(a =>a.Applicant).Include(a => a.Vacancy).ThenInclude(v => v.Business)
                .Where(a => a.ApplicantId == applicantId).ToListAsync();
        }

        public async Task<Application?> GetApplication(long id)
        {
            return await _db.Applications.Include(a => a.Applicant).Include(a => a.Vacancy).ThenInclude(v => v.Business)
                .Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Application?> ToggleChecked(long id, long businessId)
        {
            var application = await _db.Applications.Include(a => a.Vacancy).Where(a => a.Id == id).FirstOrDefaultAsync();
            if(application != null)
            {
                if (application.Vacancy.BusinessId != businessId) throw new AccessNotAllowedException();
                application.Checked = !application.Checked;
                _table.Entry(application).State = EntityState.Modified;
            }
            return application;

        }

        public async Task<Application?> ToggleApproved(long id, long businessId)
        {
            var application = await _db.Applications.Include(a => a.Vacancy).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (application != null)
            {
                if (application.Vacancy.BusinessId != businessId) throw new AccessNotAllowedException();
                application.Approved = !application.Approved;
                _table.Entry(application).State = EntityState.Modified;
            }
            return application;

        }

        public async Task<Application?> UpdateText(long id, string? text, long applicantId)
        {
            var application = await _db.Applications.FindAsync(id);
            if (application != null)
            {
                if(application.ApplicantId != applicantId) throw new AccessNotAllowedException();
                application.ApplicationText = text;
                _table.Entry(application).State = EntityState.Modified;
            }
            return application;

        }
    }
}
