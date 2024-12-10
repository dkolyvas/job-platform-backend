using AutoMapper;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class AppServices : IAppServices
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AppServices(IUnitOfWork repositories, IMapper mapper, IConfiguration configuration)
        {
            _repositories = repositories;
            _mapper = mapper;
            _configuration = configuration;
        }

        public IApplicantService ApplicantService => new ApplicantService(_repositories, _mapper);

        public IApplicationService ApplicationService => new ApplicationService(_mapper, _repositories);

        public IBusinessService BusinessService => new BusinessService(_repositories, _mapper);

        public IRegionService RegionService => new RegionService(_repositories, _mapper);

        public ISkillCategoryService SkillCategoryService => new SkillCategoryService(_repositories, _mapper);

        public ISkillLevelService SkillLevelService => new SkillLevelService(_repositories, _mapper);

        public ISkillSubcategoryService SkillSubcategoryService => new SkillSubcategoryService(_repositories, _mapper);

        public ISubscriptionService SubscriptionService => new SubscriptionService(_repositories, _mapper);

        public ISubscriptionTypeService SubscriptionTypeService => new SubscriptionTypeService(_repositories, _mapper);

        public IUserService UserService => new UserService(_repositories, _mapper, _configuration);

        public IVacancyService VacancyService => new VacancyService(_repositories, _mapper);
    }
}
