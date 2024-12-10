namespace JobPlatform.DTO.Application
{
    public class ApplicationViewDTO
    {
        public long Id { get; set; }

        public long? VacancyId { get; set; } 

        public string? VacancyName { get; set; }

        public long? BusinessId { get; set; }
        public string? BusinessName { get; set; }

        public long? ApplicantId { get; set; }

        public string? ApplicantName { get; set; }

        public string? ApplicationText { get; set; }
        public bool Checked { get; set; }

        public bool? Approved { get; set; }


        public DateOnly? ApplicationDate { get; set; }
    }
}
