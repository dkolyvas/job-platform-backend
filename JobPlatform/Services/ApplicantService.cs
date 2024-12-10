using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.Applicant;
using JobPlatform.DTO.Applicant.ApplicantMerits;
using JobPlatform.DTO.Applicant.ApplicantSkills;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;
using System.Collections.Generic;


namespace JobPlatform.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IUnitOfWork _repositories;

        private readonly IMapper _mapper;

        public ApplicantService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        public async Task<ApplicantViewExtendedDTO> AddApplicant(ApplicantInsertDTO insertDTO)
        {
            var applicant = _mapper.Map<Applicant>(insertDTO);
            List<ApplicantSkill> skills = _mapper.Map<List<ApplicantSkill>>(insertDTO.Skills);
            foreach (var skill in skills)
            {
                
                if(skill.DateFrom != null && skill.DateTo != null)
                {
                    DateOnly dateFrom = (DateOnly)skill.DateFrom;
                    DateOnly dateTo = (DateOnly)skill.DateTo;
                    int duration = (dateTo.Year - dateFrom.Year) *12 + (dateTo.Month -dateFrom.Month) + (dateTo.Day - dateFrom.Day >0 ? 1: 0); 
                    skill.DurationMonths = duration;
                }
                
            }
            List<ApplicantMerit> merits = _mapper.Map<List<ApplicantMerit>>(insertDTO.Merits);

            applicant = await _repositories.ApplicantRepository.AddOne(applicant);
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            skills = await _repositories.ApplicantSkillsRepository.AddApplicantSkills(applicant, skills);
            merits = await _repositories.ApplicantMeritsRepository.AddApplicantMerits(applicant, merits);
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return MapToExtendedDTO(applicant, skills, merits);

            
        }

        public async Task<bool> DeleteApplicant(long id)
        {
            bool result = await _repositories.ApplicantSkillsRepository.DeleteApplicantSkills(id);
            result &= await _repositories.ApplicantMeritsRepository.DeleteApplicantMerits(id);
            result &= await _repositories.ApplicantRepository.Delete(id);
            if (!result) throw new EntityNotFoundException("applicant");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<IEnumerable<ApplicantViewDTO>> FindApplicants(string? fistname = null, string? lastname = null, string? email = null)
        {
            var result = await _repositories.ApplicantRepository.FindApplicants(fistname, lastname, email);
            return _mapper.Map<IEnumerable<ApplicantViewDTO>>(result);
        }

        public async Task<ApplicantViewExtendedDTO> GetApplicantById(long id)
        {
            var applicant = await _repositories.ApplicantRepository.FindById(id);
            if (applicant == null) throw new EntityNotFoundException("applicant");
            
            List<ApplicantSkill> skills = await _repositories.ApplicantSkillsRepository.GetApplicantSkills(id);
            List<ApplicantMerit> merits = await _repositories.ApplicantMeritsRepository.GetApplicantMerits(id);
            return MapToExtendedDTO(applicant, skills, merits);
        }


        public async Task<ApplicantViewDTO?> GetApplicantByUserId(long? userId)
        {
            var applicant = await _repositories.ApplicantRepository.FindApplicantByUserId(userId);
            
            return _mapper.Map<ApplicantViewDTO>(applicant);
        }

        public async Task<ApplicantViewExtendedDTO> GetApplicantForBusiness(long applicantId, long businessId)
        {
            var applicant = await _repositories.ApplicantRepository.FindApplicantForBusiness(applicantId, businessId);
            if (applicant == null) throw new EntityNotFoundException("applicant");

            List<ApplicantSkill> skills = await _repositories.ApplicantSkillsRepository.GetApplicantSkills(applicantId);
            List<ApplicantMerit> merits = await _repositories.ApplicantMeritsRepository.GetApplicantMerits(applicantId);
            return MapToExtendedDTO(applicant, skills, merits);
        }

        public async Task<IEnumerable<ApplicantViewDTO>> GetApplicantsAsync()
        {
            var result = await _repositories.ApplicantRepository.FindAll();
            return _mapper.Map<IEnumerable<ApplicantViewDTO>>(result);
        }

        public async Task<ApplicantViewExtendedDTO> UpdateApplicant(ApplicantUpdateDTO updateDTO)
        {
            Applicant? applicant = _mapper.Map<Applicant>(updateDTO);
            List<ApplicantSkill> skills = _mapper.Map<List<ApplicantSkill>>(updateDTO.Skills);
            List<ApplicantMerit> merits = _mapper.Map<List<ApplicantMerit>>(updateDTO.Merits);

            applicant = await _repositories.ApplicantRepository.UpdateOne(applicant, applicant.Id);
            if (applicant is null) throw new EntityNotFoundException("applicant");
            skills = await _repositories.ApplicantSkillsRepository.UpdateApplicantSkills(applicant, skills);
            merits = await _repositories.ApplicantMeritsRepository.UpdateApplicantMerits(applicant, merits);

            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();

            return MapToExtendedDTO(applicant, skills, merits);
        }

        public async Task<ApplicantViewExtendedDTO> UpdateApplicantCv(long userId, string filename)
        {
            Applicant? applicant = await _repositories.ApplicantRepository.UpdateCv(filename, userId);
            if (applicant is null) throw new EntityNotFoundException("applicant");
            List<ApplicantSkill> skills = _mapper.Map<List<ApplicantSkill>>(applicant.ApplicantSkills);
            List<ApplicantMerit> merits = _mapper.Map<List<ApplicantMerit>>(applicant.ApplicantMerits);
            
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();

            return MapToExtendedDTO(applicant, skills, merits);
        }

        private ApplicantViewExtendedDTO MapToExtendedDTO(Applicant applicant, List<ApplicantSkill> skills, List<ApplicantMerit> merits)
        {
            ApplicantViewExtendedDTO result = _mapper!.Map<ApplicantViewExtendedDTO>(applicant);
            result.Skills = _mapper.Map<List<ApplicantSkillViewDTO>>(skills);
            result.Merits = _mapper.Map<List<ApplicantMeritViewDTO>>(merits);
            return result;
           
        }

        
    }
}
