using JobPlatform.DTO.Region;

namespace JobPlatform.Services
{
    public interface IRegionService
    {
        public Task<IEnumerable<RegionViewDTO>> GetAll();
        public Task<RegionViewDTO> GetById(int id);
        public Task<RegionViewDTO> AddOne(RegionInsertDTO insertDTO);
        public Task<RegionViewDTO> UpdateOne(RegionUpdateDTO updateDTO);
        public Task<bool> DeleteById(int id);
    }
}
