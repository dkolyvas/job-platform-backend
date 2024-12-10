using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.Applicant;
using JobPlatform.DTO.Application;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;
using JobPlatform.Util;

namespace JobPlatform.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repositories;

        public ApplicationService(IMapper mapper, IUnitOfWork repositories)
        {
            _mapper = mapper;
            _repositories = repositories;
        }

        /**
         * Before adding the application the function checks if the business
         * has an active subscription and if not it throws an Exception and
         * does not allow the application to be submitted. Note that 
         * only businesses with active subscription are allowed to receive
         * applications
         * */
        public async Task<ApplicationViewDTO> AddApplication(ApplicationInsertDTO insertDTO)
        {
            Application application = _mapper.Map<Application>(insertDTO);

            Vacancy? vacancy = await _repositories.VacancyRepository.FindById(insertDTO.VacancyId);
      
            if (vacancy == null) throw new EntityNotFoundException("vacancy");
            var subscription = await _repositories.SubscriptionRepository.FindMemberActiveSubscription(vacancy.BusinessId);
            if (subscription == null) throw new NoActiveSubscriptionException();
            application.ApplicationDate = DateOnly.FromDateTime(DateTime.Now);
            application.Checked = false;
            application.Approved = false;
            application = await _repositories.ApplicationRepository.AddOne(application);
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<ApplicationViewDTO>(application);
        }

        public async Task<bool> DeleteApplication(long applicationId)
        {
            var result = await _repositories.ApplicationRepository.Delete(applicationId);
            if (!result) throw new EntityNotFoundException("application");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<IEnumerable<ApplicationViewDTO>> GetApplicantApplications(long applicantId)
        {
            var data = await _repositories.ApplicationRepository.GetApplicantApplications(applicantId);
            return _mapper.Map<IEnumerable<ApplicationViewDTO>>(data);
        }

        public async Task<ApplicationViewExtendedDTO> GetApplication(long applicationId)
        {
            ApplicationViewExtendedDTO result = new();
            Application? application = await _repositories.ApplicationRepository.GetApplication(applicationId);
            if (application == null) throw new EntityNotFoundException("application");
            result = _mapper.Map<ApplicationViewExtendedDTO>(application);
            result.ApplicantAssessement = await AssessApplication(application.VacancyId, application.ApplicantId);
            return result;

        }

        public async Task<IEnumerable<ApplicationViewDTO>> GetVacancyApplications(long vacancyId, bool onlyUnchecked, bool onlyApproved)
        {
            IEnumerable<Application> data;
            if ( onlyApproved)
            {
                data = await _repositories.ApplicationRepository.GetVacacnyApprovedApplications(vacancyId);
            }
            else if (onlyUnchecked)
            {
                data = await _repositories.ApplicationRepository.GetVacancyUncheckedApplications(vacancyId);
            }
            else
            {
                data = await _repositories.ApplicationRepository.GetVacancyApplications(vacancyId);
            }
            return _mapper.Map<IEnumerable<ApplicationViewDTO>>(data);
        }

        public async Task<ApplicationViewDTO> UpdateApplication(ApplicationUpdateDTO updateDTO)
        {
            Application? application = await _repositories.ApplicationRepository.UpdateText(updateDTO.Id, updateDTO.ApplicationText, (long)updateDTO.ApplicantId!);
            if (application is null) throw new EntityNotFoundException("application");
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<ApplicationViewDTO>(application);
        }

        private int CalcultaeMonthsDifference(DateOnly? dateFrom, DateOnly? dateTo)
        {
            
            if (dateFrom is null) return 0;
            DateOnly startDate = (dateFrom is null)? DateOnly.FromDateTime(DateTime.Now): (DateOnly)dateFrom;
            DateOnly endDate = (dateTo is null) ?DateOnly.FromDateTime( DateTime.Now) : (DateOnly)dateTo;
            return (endDate.DayNumber - startDate.DayNumber) / 30;
        }

        /**
         The function checks whether the applicant matches the required criteria and assigns him a score
        according to the extent thtat he matches the required as well as the optional skills specified for the vacancy
        The overall score coefficinet is calculated as follows:
        For the required skills we calculate how many skill sorts (education, professional experience, other skills) are required and then we 
        divide  100% with this number thus finding the required skills coeficinet. This coefficient  is then multiplied for every skill sort
        with the ratio of  applicant skill duration / vacancy skill required duration (if specified) and the ratio of applicant skill grade / vacancy skill required grade (if specified)and we add the result to the overall score.
        The fulfillment of the required criteria is checked by assessing if the applicant has a required skill for education and 
        professional experience and all the required other skills and if these skills satisfy the duration and grade requirements if specified (if not the coefficient
        for duration and skillgrade is set to 1 and the matching skill is considered as fulfilling the requirement)
        
        For the optional skills we divide 70% to the number of these skills in order to find the optional skills coefficinet which will be
        multiplied by the above described coefficients for durations and sill grade.

        The controll is made by sorting out each time the vacancyskills (required and optional) for each skill sort (education,
        professional experience and other skills) and checking if the applicant has any matching skills which are further checked
        for duration and grade. If no skill subcategory is defined for the vacancy the control is made for the more general skill category of
        the applicant's specific skills.
         */
        public async Task<ApplicantAssessementDTO> AssessApplication(long? vacancyId, long? applicantId)
        {
            if (vacancyId == null || applicantId == null) return new ApplicantAssessementDTO();
            var applicantSkills = await _repositories.ApplicantSkillsRepository.GetApplicantSkills(applicantId);
            var vacancySkills = await _repositories.VacancySkillRepository.GetVacancySkills(vacancyId);
            List<VacancySkill> checklist = new();
            
            bool educationOk = false;
            bool experienceOk = false;
            bool otherSkillsOk = false;
            int applicantGrade;
            int applicantDuration;
            ApplicantSkill? matchingSkill;
            float score = 0;
            float gradeCoefficient = 0;
            float durationCoefficient = 0;
            int requiredSortsCount = vacancySkills.Select(s => s.SkillSort).Distinct().Count();
            int optionalCount = vacancySkills.Where( s => s.Required == false ).Count();
            float requiredCoefficient = (requiredSortsCount > 0 ) ? (100 /(float) requiredSortsCount) : 100;
            float optionalCoefficinet = (optionalCount > 0) ? (70 / (float)optionalCount) : 0;


            //checking for matching required education skills
            checklist = vacancySkills.Where( s => s.SkillSort == (int)Parameters.SkillSorts.Education && s.Required == true ).ToList();
            if (checklist.Count == 0)
            {
                educationOk = true;
            }
            else
            {
               foreach (VacancySkill vacancyskill in checklist)
                {
                    
                    matchingSkill = applicantSkills
                        .Where(a =>vacancyskill.SkillSubcategory!=null && a.SkillSubcategoryId == vacancyskill.SkillSubcategoryId ||
                        vacancyskill.SkillCategory != null && a.SkillSubcategory.SkillCategoryId == vacancyskill.SkillCategoryId)
                        .FirstOrDefault();
                    if (matchingSkill != null)
                    {
                        applicantDuration = CalcultaeMonthsDifference(matchingSkill.DateFrom, matchingSkill.DateTo); //(int) (matchingSkill.DurationMonths != null ? matchingSkill.DurationMonths : 0);
                        applicantGrade = (int)(matchingSkill.SkillLevel != null && matchingSkill.SkillLevel.Grade!= null? 
                            matchingSkill.SkillLevel.Grade : 1);
                        
                        gradeCoefficient = vacancyskill.SkillLevel != null && vacancyskill.SkillLevel.Grade != null  ?
                            (float)applicantGrade  / (float)vacancyskill.SkillLevel.Grade : 1;
                        durationCoefficient = vacancyskill.Duration != null ? 
                            (float)applicantDuration / (float)vacancyskill.Duration : 1;
                        if(gradeCoefficient >=1 && durationCoefficient >=1 ) educationOk = true;
                        score += requiredCoefficient * durationCoefficient * gradeCoefficient ;

                    }     
                    
                }
            }

            // checking for opotional education skills
            checklist = vacancySkills.Where(s => s.SkillSort == (int)Parameters.SkillSorts.Education && s.Required == false).ToList();
            foreach(VacancySkill vacancyskill in checklist)
            {
                matchingSkill = applicantSkills
                    .Where(a =>  vacancyskill.SkillSubcategory != null && a.SkillSubcategoryId == vacancyskill.SkillSubcategoryId ||
                        vacancyskill.SkillCategory != null && a.SkillSubcategory.SkillCategoryId == vacancyskill.SkillCategoryId)
                    .FirstOrDefault();
                if(matchingSkill != null)
                {
                    applicantDuration = CalcultaeMonthsDifference(matchingSkill.DateFrom, matchingSkill.DateTo); //(int)(matchingSkill.DurationMonths != null ? matchingSkill.DurationMonths : 0);
                    applicantGrade = (int)(matchingSkill.SkillLevel != null && matchingSkill.SkillLevel.Grade != null ?
                        matchingSkill.SkillLevel.Grade : 1);

                    gradeCoefficient = vacancyskill.SkillLevel != null && vacancyskill.SkillLevel.Grade != null ?
                        (float)applicantGrade / (float)vacancyskill.SkillLevel.Grade : 1;
                    durationCoefficient = vacancyskill.Duration != null ?
                        (float)applicantDuration / (float)vacancyskill.Duration : 1;
                    score += optionalCoefficinet * gradeCoefficient * durationCoefficient;
                }

            }
             

            //checking for matching professional experience skills
            checklist = vacancySkills.Where(s => s.SkillSort == (int)Parameters.SkillSorts.Experience && s.Required == true).ToList();
            if (checklist.Count == 0)
            {
                experienceOk = true;
            }
            else
            {
                foreach (VacancySkill vacancyskill in checklist)
                {

                    matchingSkill = applicantSkills
                        .Where(a => vacancyskill.SkillSubcategory != null && a.SkillSubcategoryId == vacancyskill.SkillSubcategoryId ||
                        vacancyskill.SkillCategory != null && a.SkillSubcategory.SkillCategoryId == vacancyskill.SkillCategoryId)
                        .FirstOrDefault();
                    if (matchingSkill != null)
                    {
                        applicantDuration = CalcultaeMonthsDifference(matchingSkill.DateFrom, matchingSkill.DateTo);// applicantDuration = (int)(matchingSkill.DurationMonths != null ? matchingSkill.DurationMonths : 0);
                        applicantGrade = (int)(matchingSkill.SkillLevel != null && matchingSkill.SkillLevel.Grade != null ?
                            matchingSkill.SkillLevel.Grade : 1);

                        gradeCoefficient = vacancyskill.SkillLevel != null && vacancyskill.SkillLevel.Grade != null ?
                            (float)applicantGrade / (float)vacancyskill.SkillLevel.Grade : 1;
                        bool hasDuration = (vacancyskill.Duration is not null);
                        durationCoefficient = hasDuration ?((float)applicantDuration / (float)vacancyskill.Duration) : 1;
                        if (gradeCoefficient >= 1 && durationCoefficient >= 1) experienceOk = true;
                        score += requiredCoefficient * durationCoefficient * gradeCoefficient;

                    }

                }
            }

            // checking for opotional professional experience skills
            checklist = vacancySkills.Where(s => s.SkillSort == (int)Parameters.SkillSorts.Experience && s.Required == false).ToList();
            foreach (VacancySkill vacancyskill in checklist)
            {
                matchingSkill = applicantSkills
                    .Where(a => vacancyskill.SkillSubcategory != null && a.SkillSubcategoryId == vacancyskill.SkillSubcategoryId ||
                        vacancyskill.SkillCategory != null && a.SkillSubcategory.SkillCategoryId == vacancyskill.SkillCategoryId)
                    .FirstOrDefault();
                if (matchingSkill != null)
                {
                    applicantDuration = CalcultaeMonthsDifference(matchingSkill.DateFrom, matchingSkill.DateTo);//applicantDuration = (int)(matchingSkill.DurationMonths != null ? matchingSkill.DurationMonths : 0);
                    applicantGrade = (int)(matchingSkill.SkillLevel != null && matchingSkill.SkillLevel.Grade != null ?
                        matchingSkill.SkillLevel.Grade : 1);

                    gradeCoefficient = vacancyskill.SkillLevel != null && vacancyskill.SkillLevel.Grade != null ?
                        (float)applicantGrade / (float)vacancyskill.SkillLevel.Grade : 1;
                    durationCoefficient = vacancyskill.Duration != null ?
                        (float)applicantDuration / (float)vacancyskill.Duration : 1;
                    score += optionalCoefficinet * gradeCoefficient * durationCoefficient;
                }

            }

            //checking for required other skills
            checklist = vacancySkills.Where(s => s.SkillSort == (int)Parameters.SkillSorts.Other && s.Required == true).ToList();
            otherSkillsOk = true;
            if (checklist.Count > 0)
            {
                foreach (VacancySkill vacancyskill in checklist)
                {

                    matchingSkill = applicantSkills
                        .Where(a => vacancyskill.SkillSubcategory != null && a.SkillSubcategoryId == vacancyskill.SkillSubcategoryId ||
                        vacancyskill.SkillCategory != null && a.SkillSubcategory.SkillCategoryId == vacancyskill.SkillCategoryId)
                        .FirstOrDefault();
                    if (matchingSkill != null)
                    {
                        applicantDuration = CalcultaeMonthsDifference(matchingSkill.DateFrom, matchingSkill.DateTo);//applicantDuration = (int)(matchingSkill.DurationMonths != null ? matchingSkill.DurationMonths : 0);
                        applicantGrade = (int)(matchingSkill.SkillLevel != null && matchingSkill.SkillLevel.Grade != null ?
                            matchingSkill.SkillLevel.Grade : 1);

                        gradeCoefficient = vacancyskill.SkillLevel != null && vacancyskill.SkillLevel.Grade != null ?
                            (float)applicantGrade / (float)vacancyskill.SkillLevel.Grade : 1;
                        durationCoefficient = vacancyskill.Duration != null ?
                            (float)applicantDuration / (float)vacancyskill.Duration : 1;
                        score += requiredCoefficient * durationCoefficient * gradeCoefficient;

                    }
                    else
                    {
                        otherSkillsOk = false;
                    }

                }
            }

            //checking for optional other skills
            checklist = vacancySkills.Where(s => s.SkillSort == (int)Parameters.SkillSorts.Other && s.Required == false).ToList();
            foreach (VacancySkill vacancyskill in checklist)
            {
                matchingSkill = applicantSkills
                    .Where(a => vacancyskill.SkillSubcategory != null && a.SkillSubcategoryId == vacancyskill.SkillSubcategoryId ||
                        vacancyskill.SkillCategory != null && a.SkillSubcategory.SkillCategoryId == vacancyskill.SkillCategoryId)
                    .FirstOrDefault();
                if (matchingSkill != null)
                {
                    applicantDuration = CalcultaeMonthsDifference(matchingSkill.DateFrom, matchingSkill.DateTo);//applicantDuration = (int)(matchingSkill.DurationMonths != null ? matchingSkill.DurationMonths : 0);
                    applicantGrade = (int)(matchingSkill.SkillLevel != null && matchingSkill.SkillLevel.Grade != null ?
                        matchingSkill.SkillLevel.Grade : 1);

                    gradeCoefficient = vacancyskill.SkillLevel != null && vacancyskill.SkillLevel.Grade != null ?
                        (float)applicantGrade / (float)vacancyskill.SkillLevel.Grade : 1;
                    durationCoefficient = vacancyskill.Duration != null ?
                        (float)applicantDuration / (float)vacancyskill.Duration : 1;
                    score += optionalCoefficinet * gradeCoefficient * durationCoefficient;
                }
                
            }
            return new ApplicantAssessementDTO { Score = score , IsMatching = educationOk & experienceOk & otherSkillsOk};
            
        }

       /* public async Task<IEnumerable<ApplicationViewDTO>> GetVacancyUncheckedApplications(long vacancyId)
        {
            var data = await _repositories.ApplicationRepository.GetVacancyUncheckedApplications(vacancyId);
            return _mapper.Map<IEnumerable<ApplicationViewDTO>>(data);
        }

        public async Task<IEnumerable<ApplicationViewDTO>> GetVacancyApprovedApplications(long vacancyId)
        {
            var data = await _repositories.ApplicationRepository.GetVacacnyApprovedApplications(vacancyId);
            return _mapper.Map<IEnumerable<ApplicationViewDTO>>(data);
        }*/

        public async Task<ApplicationViewDTO> ToggleApproved(long applicationId, long businessId)
        {
            var application =await _repositories.ApplicationRepository.ToggleApproved(applicationId, businessId);
            if (application is null) throw new EntityNotFoundException("application");
            if(!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<ApplicationViewDTO>(application);   
        }

        public async Task<ApplicationViewDTO> ToggleChecked(long applicationId, long businessId)
        {
            var application = await _repositories.ApplicationRepository.ToggleChecked(applicationId, businessId);
            if (application is null) throw new EntityNotFoundException("application");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<ApplicationViewDTO>(application);
        }
    }
}
