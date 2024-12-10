namespace JobPlatform.Services
{
    public interface IAppServices
    {
        public IApplicantService ApplicantService { get; }
        public IApplicationService ApplicationService { get; }
        public IBusinessService BusinessService { get; }
        public IRegionService RegionService { get; }
        public ISkillCategoryService SkillCategoryService { get; }
        public ISkillLevelService SkillLevelService { get; }
        public ISkillSubcategoryService SkillSubcategoryService { get; }
        public ISubscriptionService SubscriptionService { get; }
        public ISubscriptionTypeService SubscriptionTypeService { get; }
        public IUserService UserService { get; }
        public IVacancyService VacancyService { get; }
    }
}
