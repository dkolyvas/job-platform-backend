using JobPlatform.DTO.SubscriptionType;

namespace JobPlatform.Services
{
    public interface ISubscriptionTypeService
    {
        public Task<IEnumerable<SubscriptionTypeViewDTO>> GetAll();
        public Task<SubscriptionTypeViewDTO> GetById(int id);
        public Task<SubscriptionTypeViewDTO> AddOne(SubscriptionTypeInsertDTO insertDTO);
        public Task<SubscriptionTypeViewDTO> UpdateOne(SubscriptionTypeUpdateDTO updateDTO);
        public Task<bool> DeleteById(int id);
    }
}
