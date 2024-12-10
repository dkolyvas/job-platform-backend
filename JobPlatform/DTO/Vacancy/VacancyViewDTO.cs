using JobPlatform.DTO.Vacancy.VacancyMerits;
using JobPlatform.DTO.Vacancy.VacancySkills;

namespace JobPlatform.DTO.Vacancy
{
    public class VacancyViewDTO
    {
        public long Id { get; set; }

        public long? BusinessId { get; set; }

        public string? BusinessName {  get; set; }

        public bool? Active { get; set; }

        public string? Address { get; set; }

        public string? Description { get; set; }

        public string? Title { get; set; } = null!;

        public long SkillSubcategoryId { get; set; }
        public string? SkillSubcategoryName { get; set; }

        public int RegionId { get; set; }
        public string? RegionName { get; set; }

        public DateOnly? PublicationDate { get; set; }

      
    }
}
