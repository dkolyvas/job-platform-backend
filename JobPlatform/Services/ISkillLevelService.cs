using JobPlatform.DTO.Skill_Level;

namespace JobPlatform.Services
{
    public interface ISkillLevelService
    {
        public Task<IEnumerable<SkillLevelViewDTO>> FindSkillLevelsForSort(int sort);
        public Task<IEnumerable<SkillLevelViewDTO>> FindSkillLevelsForCategory(int categoryId);
        public Task<IEnumerable<SkillLevelViewDTO>> FindSkillLevelsForSubcategory(long subcategoryId);
        public Task<SkillLevelViewDTO> FindById(int id);
        public Task<SkillLevelViewDTO> AddLevel(SkillLevelInsertDTO insertDTO);
        public Task<SkillLevelViewDTO> UpdateLevel(SkillLevelUpdateDTO updateDTO);
        public Task<bool> Delete(int id);
    }
}
