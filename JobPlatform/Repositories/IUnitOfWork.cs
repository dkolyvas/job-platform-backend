using JobPlatform.Data;

namespace JobPlatform.Repositories
{
    public interface IUnitOfWork
    {
        public ApplicantMeritsRepository ApplicantMeritsRepository { get; }
        public ApplicantRepository ApplicantRepository { get; }
        public ApplicantSkillsRepository ApplicantSkillsRepository { get; }
        public ApplicationRepository ApplicationRepository { get; }
        public BusinessRepository BusinessRepository { get; }
        public SkillCategoryRepository SkillCategoryRepository { get; }
        public SkillLevelRepository SkillLevelRepository { get; }
        public SkillSubcategoryRepository SkillSubcategoryRepository { get; }
        public SubscriptionRepository SubscriptionRepository { get; }
        public VacancyMeritRepository   VacancyMeritRepository { get; }
        public VacancyRepository VacancyRepository { get; }
        public VacancySkillRepository VacancySkillRepository { get; }
        public UserRepository UserRepository { get; }
        public RegionRepository RegionsRepository { get; }
        public SubscriptionTypeRepository SubscriptionTypesRepository { get; }


        public Task<bool> SaveChanges();
    }
}
