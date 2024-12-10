namespace JobPlatform.DTO.Application
{
    public class ApplicationViewExtendedDTO
    {
        public long Id { get; set; }

        public long? VacancyId { get; set; }

        public string? VacancyName { get; set; }

        public long BusinessId { get; set; } = -1;
        public string? BusinessName { get; set; }

        public long? ApplicantId { get; set; }

        public string? ApplicantName { get; set; }

        public string? ApplicationText { get; set; }

        public ApplicantAssessementDTO? ApplicantAssessement { get; set; }
    }
}
