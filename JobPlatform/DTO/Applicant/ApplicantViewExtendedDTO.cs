using JobPlatform.DTO.Applicant.ApplicantMerits;
using JobPlatform.DTO.Applicant.ApplicantSkills;

namespace JobPlatform.DTO.Applicant
{
    public class ApplicantViewExtendedDTO
    {
        public long Id { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Username { get; set; }

        public string? Cv { get; set; }

        public List<ApplicantSkillViewDTO> Skills { get; set; } = new();

        public List<ApplicantMeritViewDTO> Merits { get; set; } = new();
    }
}
