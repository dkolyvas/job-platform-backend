using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.SkillCategory;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class SkillCategoryService : ISkillCategoryService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;

        public SkillCategoryService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        public async Task<SkillCategoryViewDTO> Add(SkillCategoryInsertDTO insertDTO)
        {
            SkillCategory category = _mapper.Map<SkillCategory>(insertDTO);
            category = await _repositories.SkillCategoryRepository.AddOne(category);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SkillCategoryViewDTO>(category);
        }

        

        public async Task<IEnumerable<SkillCategoryViewDTO>> GetSkillCategoriesAsync(int skillSort)
        {
            var data = await _repositories.SkillCategoryRepository.FindCategoriesBySort(skillSort);
            return _mapper.Map<IEnumerable<SkillCategoryViewDTO>>(data);
        }

        public async Task<SkillCategoryViewDTO> GetSkillCategory(int id)
        {
            var data = await _repositories.SkillCategoryRepository.FindById(id);
            if (data is null) throw new EntityNotFoundException("skill category");
            return _mapper.Map<SkillCategoryViewDTO>(data);
        }

        public async Task<int> MergeCategories(int mergedCategoryId, int remainingCategoryId)
        {
            int result = await _repositories.SkillCategoryRepository.MergeCategories(mergedCategoryId, remainingCategoryId);
            if(result == 0)
            {
                throw new EntityNotFoundException("skill category");
            }
            else
            {
                return result;
            }
        }

        public async Task<SkillCategoryViewDTO> Update(SkillCategoryUpdateDTO updateDTO)
        {
            SkillCategory category = _mapper.Map<SkillCategory>(updateDTO);
            category = await _repositories.SkillCategoryRepository.UpdateOne(category);
            if (category is null) throw new EntityNotFoundException("skill category");
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SkillCategoryViewDTO>(category);
        }
    }
}
