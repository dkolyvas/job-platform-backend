using JobPlatform.DTO.SkillCategory;

namespace JobPlatform.Services
{
    public interface ISkillCategoryService
    {
        public Task<IEnumerable<SkillCategoryViewDTO>> GetSkillCategoriesAsync(int skillSort);
        public Task<SkillCategoryViewDTO> GetSkillCategory(int id);
        public Task<SkillCategoryViewDTO> Add(SkillCategoryInsertDTO insertDTO);
        public Task<SkillCategoryViewDTO> Update(SkillCategoryUpdateDTO updateDTO);

        public Task<int> MergeCategories(int mergedCategoryId, int remainingCategoryId);
    }
}
