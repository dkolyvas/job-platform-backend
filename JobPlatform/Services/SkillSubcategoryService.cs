using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.SkillSubacategory;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class SkillSubcategoryService : ISkillSubcategoryService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;

        public SkillSubcategoryService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        public async Task<SkillSubcategoryViewDTO> Add(SkillSubcategoryInsertDTO insertDTO)
        {
            SkillSubcategory subcategory = _mapper.Map<SkillSubcategory>(insertDTO);
            subcategory.Checked = false;
            subcategory = await _repositories.SkillSubcategoryRepository.AddOne(subcategory);
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SkillSubcategoryViewDTO>(subcategory);
        }

        public async Task<IEnumerable<SkillSubcategoryViewDTO>> GetByCategory(int categoryId)
        {
            var data = await _repositories.SkillSubcategoryRepository.FindSubcategories(categoryId);
            return _mapper.Map<IEnumerable<SkillSubcategoryViewDTO>>(data);
        }

        public async Task<SkillSubcategoryViewDTO> GetById(long id)
        {
            var data = await _repositories.SkillSubcategoryRepository.FindById(id);
            if (data is null) throw new EntityNotFoundException("skill subcategory");
            return _mapper.Map<SkillSubcategoryViewDTO>(data);
        }

        public async Task<IEnumerable<SkillSubcategoryViewDTO>> GetUnchecked()
        {
            var data = await _repositories.SkillSubcategoryRepository.FindUnchecked();
            return _mapper.Map<IEnumerable<SkillSubcategoryViewDTO>>(data);
        }

        public async Task<SkillSubcategoryViewDTO> MarkAsChecked(long subcategoryId)
        {
            var subcategory = await _repositories.SkillSubcategoryRepository.MarkAsChecked(subcategoryId);
            if (subcategory is null) throw new EntityNotFoundException("skill subcategory");
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SkillSubcategoryViewDTO>(subcategory);
        }

        public async Task<int> MergeSubcategories(long mergedSubcategoryId, long remainingSubacateogoryId)
        {
            int result = await _repositories.SkillSubcategoryRepository.MergeSubcategories(mergedSubcategoryId, remainingSubacateogoryId);
            if (result == 0) throw new EntityNotFoundException("skill subcategory");
            return result; 
        }

        public async Task<SkillSubcategoryViewDTO> Update(SkillSubcategoryUpdateDTO updateDTO)
        {
            SkillSubcategory? subcategory= _mapper.Map<SkillSubcategory>(updateDTO);
            subcategory =await  _repositories.SkillSubcategoryRepository.UpdateSubcategory(subcategory);
            if (subcategory is null) throw new EntityNotFoundException("skill subcategory");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SkillSubcategoryViewDTO>(subcategory);
        }
    }
}
