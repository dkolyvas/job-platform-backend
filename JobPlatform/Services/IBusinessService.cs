using JobPlatform.DTO.Business;

namespace JobPlatform.Services
{
    public interface IBusinessService
    {
        Task<IEnumerable<BusinessViewDTO>> GetAll();
        Task<IEnumerable<BusinessViewDTO>> GetByNameOrEmail(string? name = null, string? email = null );
        Task<BusinessViewDTO?> GetByUserId(long userid);
        Task<BusinessViewDTO> GetById(long id);
        Task<BusinessViewDTO> Insert(BusinessInsertDTO businessInsertDTO);
        Task<BusinessViewDTO> Update(BusinessUpdateDTO businessUpdateDTO);
        Task<BusinessViewDTO> UpdateImage(BusinessUpdateImageDTO updateImageDTO);
        Task<bool> Delete(long id);

    }
}
