using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.SearchCriteria;
using JobPlatform.DTO.Vacancy;
using JobPlatform.DTO.Vacancy.VacancyMerits;
using JobPlatform.DTO.Vacancy.VacancySkills;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;

        public VacancyService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        /**
         * The method adds a new vacancy along with its associated skills and merits.
         * Before doing that it checks whether the business has an active subscription
         * and it has not exceeded its vacancy post limit specified by the subscription's allowance
         */

        public async Task<VacancyViewExtendedDTO> AddVacancy(VacancyInsertDTO insertDTO)
        {
            Vacancy vacancy = _mapper.Map<Vacancy>(insertDTO);
            vacancy.PublicationDate =   DateOnly.FromDateTime(DateTime.Now);
            vacancy.Active = true;
            var skills = _mapper.Map<List<VacancySkill>>(insertDTO.Skills);
            var merits = _mapper.Map<List<VacancyMerit>>(insertDTO.Merits);
            var subscription = await _repositories.SubscriptionRepository.FindMemberActiveSubscription(insertDTO.BusinessId);
            if (subscription is null) throw new NoActiveSubscriptionException();
            int currentPosts = await _repositories.VacancyRepository.FindAnnouncementsSinceDate(subscription.StartDate);
            if(currentPosts >= subscription.Allowance) throw new AllowanceExceededException();

            vacancy = await _repositories.VacancyRepository.AddOne(vacancy);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            skills = await _repositories.VacancySkillRepository.AddVacancySkills(vacancy, skills);
            merits = await _repositories.VacancyMeritRepository.AddVacancyMerits(vacancy, merits);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();

            return MapToExtendedDTO(vacancy, skills, merits);
        }

        public async Task<bool> DeleteVacancyById(long id)
        {
            var result = await _repositories.VacancyRepository.Delete(id);
            if (!result) throw new EntityNotFoundException("vacancy");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<VacancyViewDTO> DisactivateVacancy(long id)
        {
            Vacancy? vacancy = await _repositories.VacancyRepository.DisactivateVacancy(id);
            if (vacancy == null) throw new EntityNotFoundException("vacancy");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<VacancyViewDTO>(vacancy);
        }

        public async Task<VacancyViewDTO> DisactivateVacancy(long id, long businessId)
        {
            Vacancy? vacancy = await _repositories.VacancyRepository.DisactivateVacancy(id, businessId);
            if (vacancy == null) throw new EntityNotFoundException("vacancy");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<VacancyViewDTO>(vacancy);

        }

        public async Task<IEnumerable<VacancyViewDTO>> FindBusinessVacancies(long businesId)
        {
            var vacancies = await _repositories.VacancyRepository.GetBusinessVacancies(businesId);
            return _mapper.Map<List<VacancyViewDTO>>(vacancies);    
        }

        public async Task<IEnumerable<VacancyViewDTO>> FindVacancies(VacancySearchCriteria searchCriteria)
        {
            foreach(var skill in searchCriteria.SkillSearchCriteria)
            {
                if(skill.CategoryId is null && skill.SubcategoryId is not null)
                {
                    var skillSubcategory = await _repositories.SkillSubcategoryRepository.FindById((long)skill.SubcategoryId);
                    skill.CategoryId = skillSubcategory != null?skillSubcategory.SkillCategoryId:-1;
                }
            }
            var vacancies = await _repositories.VacancyRepository.FindVacanies(searchCriteria);
            return _mapper.Map<List<VacancyViewDTO>>(vacancies);
        }

        public async Task<IEnumerable<VacancyViewDTO>> GetAll()
        {
            var vacancies = await _repositories.VacancyRepository.FindAll();
            return _mapper.Map<List<VacancyViewDTO>>(vacancies);
        }

        public async Task<VacancyViewExtendedDTO> GetVacancyById(long id)
        {
            var vacancy = await _repositories.VacancyRepository.FindById(id);
            if (vacancy == null) throw new EntityNotFoundException("vacancy");
            var merits = await _repositories.VacancyMeritRepository.GetVacancyMerits(id);
            var skills = await _repositories.VacancySkillRepository.GetVacancySkills(id);
            return MapToExtendedDTO(vacancy, skills, merits);
            
        }

        public async Task<VacancyViewExtendedDTO> UpdateVacancy(VacancyUpdateDTO updateDTO)
        {
            Vacancy? vacancy = _mapper.Map<Vacancy>(updateDTO);
            List<VacancySkill> skills = _mapper.Map<List<VacancySkill>>(updateDTO.Skills);
            List<VacancyMerit> merits = _mapper.Map<List<VacancyMerit>>(updateDTO.Merits);
            vacancy = await  _repositories.VacancyRepository.UpdateVacancy(vacancy);
            if (vacancy == null) throw new EntityNotFoundException("vacancy");
            skills = await _repositories.VacancySkillRepository.UpdateVacancySkills(vacancy, skills);
            merits = await _repositories.VacancyMeritRepository.UpdateVacancyMerits(vacancy, merits);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return MapToExtendedDTO(vacancy, skills, merits);

        }

        private VacancyViewExtendedDTO MapToExtendedDTO(Vacancy vacancy, List<VacancySkill> skills, List<VacancyMerit> merits)
        {
            VacancyViewExtendedDTO result = _mapper.Map<VacancyViewExtendedDTO>(vacancy);
            result.Skills = _mapper.Map<List<VacancySkillViewDTO>>(skills);
            result.Merits = _mapper.Map<List<VacancyMeritViewDTO>>(merits);
            return result;
        }
    }
}
