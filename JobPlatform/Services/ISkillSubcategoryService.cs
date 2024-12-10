using JobPlatform.DTO.SkillSubacategory;

namespace JobPlatform.Services
{
    public interface ISkillSubcategoryService
    {
        public Task<IEnumerable<SkillSubcategoryViewDTO>> GetByCategory(int categoryId);
        public Task<IEnumerable<SkillSubcategoryViewDTO>> GetUnchecked();
        public Task<SkillSubcategoryViewDTO> GetById(long id);
        public Task<SkillSubcategoryViewDTO> Add(SkillSubcategoryInsertDTO insertDTO);
        public Task<SkillSubcategoryViewDTO> Update(SkillSubcategoryUpdateDTO updateDTO);
        public Task<int> MergeSubcategories(long mergedSubcategoryId, long remainingSubacateogoryId);

        public Task<SkillSubcategoryViewDTO> MarkAsChecked(long subcategoryId);
    }
}
