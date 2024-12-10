using JobPlatform.Data;
using JobPlatform.Exceptions;

namespace JobPlatform.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private JobplatformContext _db;

        public UnitOfWork(JobplatformContext db)
        {
            _db = db;
        }

        public ApplicantMeritsRepository ApplicantMeritsRepository => new ApplicantMeritsRepository(_db);

        public ApplicantRepository ApplicantRepository => new ApplicantRepository(_db);

        public ApplicantSkillsRepository ApplicantSkillsRepository => new ApplicantSkillsRepository(_db);

        public ApplicationRepository ApplicationRepository => new ApplicationRepository(_db);

        public BusinessRepository BusinessRepository => new BusinessRepository(_db);

        public SkillCategoryRepository SkillCategoryRepository => new SkillCategoryRepository(_db);

        public SkillLevelRepository SkillLevelRepository => new SkillLevelRepository(_db);

        public SkillSubcategoryRepository SkillSubcategoryRepository => new SkillSubcategoryRepository(_db);

        public SubscriptionRepository SubscriptionRepository => new SubscriptionRepository(_db);

        public VacancyMeritRepository VacancyMeritRepository => new VacancyMeritRepository(_db);

        public VacancyRepository VacancyRepository => new VacancyRepository(_db);

        public VacancySkillRepository VacancySkillRepository => new VacancySkillRepository(_db);

        public UserRepository UserRepository => new UserRepository(_db);

        public RegionRepository RegionsRepository => new RegionRepository(_db);

        public SubscriptionTypeRepository SubscriptionTypesRepository => new SubscriptionTypeRepository(_db);

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _db.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                throw new UnableToSaveDataException(ex.Message);
            }
        }
    }
}
