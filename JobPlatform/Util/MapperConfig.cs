using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO;
using JobPlatform.DTO.Applicant;
using JobPlatform.DTO.Applicant.ApplicantMerits;
using JobPlatform.DTO.Applicant.ApplicantSkills;
using JobPlatform.DTO.Application;
using JobPlatform.DTO.Business;
using JobPlatform.DTO.Region;
using JobPlatform.DTO.Skill_Level;
using JobPlatform.DTO.SkillCategory;
using JobPlatform.DTO.SkillSubacategory;
using JobPlatform.DTO.Subscription;
using JobPlatform.DTO.SubscriptionType;
using JobPlatform.DTO.User;
using JobPlatform.DTO.Vacancy;
using JobPlatform.DTO.Vacancy.VacancyMerits;
using JobPlatform.DTO.Vacancy.VacancySkills;

namespace JobPlatform.Util
{
    public class MapperConfig : Profile
    {
        private readonly string[] _skillSorts = { "Education", "Professional Experience", "Other Skills" };
        public MapperConfig()
        {
            CreateMap<Business, BusinessInsertDTO>().ReverseMap();
            CreateMap<Business, BusinessUpdateDTO>().ReverseMap();
            CreateMap<Business, BusinessViewDTO>().ForMember(d => d.Username, f => f.MapFrom(s => s.User.Username));
            CreateMap<ApplicantSkill, ApplicantSkillInsertDTO>().ReverseMap();
            CreateMap<ApplicantSkill, ApplicantSkillViewDTO>()
                .ForMember(d => d.SkillSubcategoryName, f => f.MapFrom(s => s.SkillSubcategory.Name))
                .ForMember(d => d.SkillCategoryId, f => f.MapFrom(s => s.SkillSubcategory.SkillCategoryId))
                .ForMember(d => d.SkillCategoryName, f => f.MapFrom(s => s.SkillSubcategory.SkillCategory.Name))
                .ForMember(d => d.SkillLevelName, f => f.MapFrom(s =>s.SkillLevel !=null? s.SkillLevel.Name: null));
           //     .ForMember(d => d.SkillGrade, f => f.MapFrom(s => s.SkillLevel.Grade));
            CreateMap<ApplicantMerit, ApplicantMeritInsertDTO>().ReverseMap();
            CreateMap<ApplicantMerit, ApplicantMeritViewDTO>().ReverseMap();
            CreateMap<Applicant, ApplicantInsertDTO>().ReverseMap();
            CreateMap<Applicant, ApplicantUpdateDTO>().ReverseMap();
            CreateMap<Applicant, ApplicantViewDTO>().ForMember(d => d.Username, f => f.MapFrom(s => s.User.Username));
            CreateMap<Applicant, ApplicantViewExtendedDTO>().ForMember(d => d.Username, f => f.MapFrom(s => s.User.Username));
            CreateMap<VacancyMerit, VacancyMeritInsertDTO>().ReverseMap();
            CreateMap<VacancyMerit, VacancyMeritViewDTO>().ReverseMap();
            CreateMap<VacancySkill, VacancySkillInsertDTO>().ReverseMap();
            CreateMap<VacancySkill, VacancySkillViewDTO>()
                .ForMember(s => s.SkillCategoryId, f => f.MapFrom(d => d.SkillCategoryId != null ? d.SkillCategoryId :(d.SkillSubcategory !=null? d.SkillSubcategory.SkillCategoryId: null )))
                .ForMember(s => s.SkillCategoryName, f => f.MapFrom(d => d.SkillCategory != null  ? d.SkillCategory.Name:(d.SkillSubcategory !=null? d.SkillSubcategory.SkillCategory.Name: null)))
                .ForMember(s => s.SkillSubcategoryName, f => f.MapFrom(d =>d.SkillSubcategory !=null? d.SkillSubcategory.Name: null))
                .ForMember(s => s.SkillLevelName, f => f.MapFrom(d =>d.SkillLevel !=null? d.SkillLevel.Name: null));
            CreateMap<Vacancy, VacancyInsertDTO>().ReverseMap();
            CreateMap<Vacancy, VacancyUpdateDTO>().ReverseMap();
            CreateMap<Vacancy, VacancyViewDTO>()
                .ForMember(s => s.BusinessName, f => f.MapFrom(d => d.Business.Name))
                .ForMember(s => s.SkillSubcategoryName, f => f.MapFrom(d => d.SkillSubcategory.Name))
                .ForMember(s => s.RegionName, f => f.MapFrom(d => d.Region.Name));
            
            CreateMap<Vacancy, VacancyViewExtendedDTO>()
                .ForMember(s => s.BusinessName, f => f.MapFrom(d => d.Business.Name))
                .ForMember(s => s.SkillSubcategoryName, f => f.MapFrom(d => d.SkillSubcategory.Name))
                .ForMember(s => s.RegionName, f => f.MapFrom(d => d.Region.Name));
            CreateMap<Application, ApplicationInsertDTO>().ReverseMap();
            CreateMap<Application, ApplicationUpdateDTO>().ReverseMap();
            CreateMap<Application, ApplicationViewDTO>()
                .ForMember( d => d.BusinessId, f => f.MapFrom(s => s.Vacancy !=null? s.Vacancy.BusinessId: null))
                .ForMember( d => d.BusinessName, f => f.MapFrom(s => s.Vacancy !=null && s.Vacancy.Business != null?  s.Vacancy.Business.Name: ""))
                .ForMember( d => d.VacancyName, f => f.MapFrom(s => s.Vacancy != null ? s.Vacancy.Title: ""))
                .ForMember( d=> d.ApplicantName, f => f.MapFrom(s => s.Applicant != null ? s.Applicant.Firstname + " " + s.Applicant.Lastname: ""));
            CreateMap<Application, ApplicationViewExtendedDTO>()
                .ForMember(d => d.BusinessId, f => f.MapFrom(s => s.Vacancy != null ? s.Vacancy.BusinessId : null))
                .ForMember(d => d.BusinessName, f => f.MapFrom(s => s.Vacancy != null && s.Vacancy.Business != null ? s.Vacancy.Business.Name : ""))
                .ForMember(d => d.VacancyName, f => f.MapFrom(s => s.Vacancy != null ? s.Vacancy.Title : ""))
                .ForMember(d => d.ApplicantName, f => f.MapFrom(s => s.Applicant != null ? s.Applicant.Firstname + " " + s.Applicant.Lastname : ""));
            CreateMap<SkillCategory, SkillCategoryInsertDTO>().ReverseMap();
            CreateMap<SkillCategory, SkillCategoryUpdateDTO>().ReverseMap();
            CreateMap<SkillCategory, SkillCategoryViewDTO>()
                .ForMember(d => d.SortName, f => f.MapFrom(s => (s.Sort != null && s.Sort >= 0 && s.Sort < 3) ?
                _skillSorts[(int)s.Sort] : null));
            CreateMap<SkillSubcategory, SkillSubcategoryInsertDTO>().ReverseMap();
            CreateMap<SkillSubcategory, SkillSubcategoryUpdateDTO>().ReverseMap();
            CreateMap<SkillSubcategory, SkillSubcategoryViewDTO>()
                .ForMember(d => d.SortName, f => f.MapFrom(s => (s.SkillCategory.Sort != null && s.SkillCategory.Sort >= 0 && s.SkillCategory.Sort < 3) ?
                _skillSorts[(int)s.SkillCategory.Sort] : null))
                .ForMember(d => d.CategoryName, f => f.MapFrom(s => s.SkillCategory.Name));
            CreateMap<SkillLevel, SkillLevelInsertDTO>().ReverseMap();
            CreateMap<SkillLevel, SkillLevelUpdateDTO>().ReverseMap();
            CreateMap<SkillLevel, SkillLevelViewDTO>()
                .ForMember(d => d.SortName, f => f.MapFrom(s => (s.SkillSort != null && s.SkillSort >= 0 && s.SkillSort < 3) ?
                _skillSorts[(int)s.SkillSort] : null))
                .ForMember(d => d.CategoryName, f => f.MapFrom(s => s.SkillCategory.Name))
                .ForMember(d => d.SubcategoryName, f => f.MapFrom(s => s.SkillSubcategory.Name));
            CreateMap<Subscription, SubscriptionViewDTO>()
                .ForMember( d => d.BusinessName, f => f.MapFrom(s => s.Business.Name));
            CreateMap<Subscription, SubscriptionUpdateDTO>().ReverseMap();
            CreateMap<Region, RegionViewDTO>().ReverseMap();
            CreateMap<Region, RegionInsertDTO>().ReverseMap();
            CreateMap<Region, RegionUpdateDTO>().ReverseMap();
            CreateMap<SubscriptionType, SubscriptionTypeViewDTO>().ReverseMap();
            CreateMap<SubscriptionType, SubscriptionTypeInsertDTO>().ReverseMap();
            CreateMap<SubscriptionType, SubscriptionTypeUpdateDTO>().ReverseMap();
            CreateMap<User, UserViewDTO>().ReverseMap();


        }
    }
}
