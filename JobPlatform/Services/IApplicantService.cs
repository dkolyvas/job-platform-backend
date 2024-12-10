using JobPlatform.DTO.Applicant;

namespace JobPlatform.Services
{
    public interface IApplicantService
    {
        public Task<IEnumerable<ApplicantViewDTO>> GetApplicantsAsync();
        public Task<IEnumerable<ApplicantViewDTO>> FindApplicants(string? fistname = null, string? lastname = null, string? email = null);
        public Task<ApplicantViewDTO?> GetApplicantByUserId(long? userId);

        public Task<ApplicantViewExtendedDTO> GetApplicantById(long id);
        public Task<ApplicantViewExtendedDTO> GetApplicantForBusiness(long applicantId, long businessId);

        public Task<ApplicantViewExtendedDTO> AddApplicant(ApplicantInsertDTO insertDTO);

        public Task<ApplicantViewExtendedDTO> UpdateApplicant(ApplicantUpdateDTO updateDTO);

        public Task<ApplicantViewExtendedDTO> UpdateApplicantCv(long userId, string filename);

        public Task<bool> DeleteApplicant(long id);  

        
    }
}
