using JobPlatform.DTO.Application;

namespace JobPlatform.Services
{
    public interface IApplicationService
    {
        public Task<IEnumerable<ApplicationViewDTO>> GetApplicantApplications(long applicantId);

        public Task<IEnumerable<ApplicationViewDTO>> GetVacancyApplications(long vacancyId, bool onlyUnchecked, bool onlyApproved);

        

        public Task<ApplicationViewExtendedDTO> GetApplication(long applicationId);

        public Task<ApplicationViewDTO> AddApplication(ApplicationInsertDTO insertDTO);

        public Task<ApplicationViewDTO> UpdateApplication(ApplicationUpdateDTO updateDTO);

        public Task<ApplicationViewDTO> ToggleApproved(long applicationId, long businessId);
        
        public Task<ApplicationViewDTO> ToggleChecked(long applicationId, long businessId);

        public Task<bool> DeleteApplication(long applicationId);
        public Task<ApplicantAssessementDTO> AssessApplication(long? vacancyId, long? applicantId);
    }
}
