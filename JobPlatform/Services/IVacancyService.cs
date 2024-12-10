using JobPlatform.DTO.SearchCriteria;
using JobPlatform.DTO.Vacancy;

namespace JobPlatform.Services
{
    public interface IVacancyService
    {
        public Task<IEnumerable<VacancyViewDTO>> GetAll();

        public Task<IEnumerable<VacancyViewDTO>> FindVacancies(VacancySearchCriteria searchCriteria);

        public Task<IEnumerable<VacancyViewDTO>> FindBusinessVacancies(long businessId);

        public Task<VacancyViewExtendedDTO> GetVacancyById(long id);

        public Task<bool> DeleteVacancyById(long id);

        public Task<VacancyViewExtendedDTO> AddVacancy(VacancyInsertDTO insertDTO);

        public Task<VacancyViewExtendedDTO> UpdateVacancy(VacancyUpdateDTO updateDTO);

        public Task<VacancyViewDTO> DisactivateVacancy(long id);

        public Task<VacancyViewDTO> DisactivateVacancy(long id, long businessId);


    }
}
