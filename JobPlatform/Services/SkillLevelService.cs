using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.Skill_Level;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class SkillLevelService : ISkillLevelService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;

        public SkillLevelService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        public async Task<SkillLevelViewDTO> AddLevel(SkillLevelInsertDTO insertDTO)
        {
            SkillLevel level = _mapper.Map<SkillLevel>(insertDTO);
            level = await _repositories.SkillLevelRepository.AddOne(level);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SkillLevelViewDTO>(level);
        }

        public async Task<bool> Delete(int id)
        {
            bool result = await _repositories.SkillLevelRepository.Delete(id);
            if (!result) throw new EntityNotFoundException("skill level");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<SkillLevelViewDTO> FindById(int id)
        {
            var level = await _repositories.SkillLevelRepository.FindById(id);
            if (level is null) throw new EntityNotFoundException("skill level");
            return _mapper.Map<SkillLevelViewDTO>(level);
        }

        public async Task<IEnumerable<SkillLevelViewDTO>> FindSkillLevelsForCategory(int categoryId)
        {
            var results = await _repositories.SkillLevelRepository.FindSkillLevelsForCategory(categoryId);
            return _mapper.Map<IEnumerable<SkillLevelViewDTO>>(results);
        }

        public async Task<IEnumerable<SkillLevelViewDTO>> FindSkillLevelsForSort(int sort)
        {
            var results = await _repositories.SkillLevelRepository.FindSkillLevelsForSort(sort);
            return _mapper.Map<IEnumerable<SkillLevelViewDTO>>(results);
        }

        public async Task<IEnumerable<SkillLevelViewDTO>> FindSkillLevelsForSubcategory(long subcategoryId)
        {
            var results = await _repositories.SkillLevelRepository.FindSkillLevelsForSubcategory(subcategoryId);
            return _mapper.Map<IEnumerable<SkillLevelViewDTO>>(results);
        }

        public async Task<SkillLevelViewDTO> UpdateLevel(SkillLevelUpdateDTO updateDTO)
        {
            SkillLevel? level = _mapper.Map<SkillLevel>(updateDTO);
            level = await _repositories.SkillLevelRepository.UpdateOne(level);
            if (level is null) throw new EntityNotFoundException("skill level");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SkillLevelViewDTO>(level);
        }
    }
}
