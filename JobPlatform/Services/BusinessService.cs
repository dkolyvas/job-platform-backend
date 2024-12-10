using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.Business;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class BusinessService : IBusinessService
    {
        private IUnitOfWork _repositories;
        private IMapper _mapper;

        public BusinessService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

       

        public async Task<bool> Delete(long id)
        {
            bool result = await _repositories.BusinessRepository.Delete(id);
            if (!result) throw new EntityNotFoundException("business");
            if(!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<IEnumerable<BusinessViewDTO>> GetAll()
        {
            var data = await _repositories.BusinessRepository.FindAll();
            return _mapper.Map<List<BusinessViewDTO>>(data);
        }

        public async Task<BusinessViewDTO> GetById(long id)
        {
            var data = await _repositories.BusinessRepository.FindById(id);
            if (data is null) throw new EntityNotFoundException("business");
            return _mapper.Map<BusinessViewDTO>(data);
        }

        public async Task<IEnumerable<BusinessViewDTO>> GetByNameOrEmail(string? name = null, string? email = null)
        {
            var data = await _repositories.BusinessRepository.FindBusinessByName(name, email);
            return _mapper.Map<List<BusinessViewDTO>>(data);  
        }

        

        public async Task<BusinessViewDTO?> GetByUserId(long userid)
        {
            var data = await _repositories.BusinessRepository.FindBusinessByUser(userid);
            return _mapper.Map<BusinessViewDTO>(data);
        }

        public async Task<BusinessViewDTO> Insert(BusinessInsertDTO businessInsertDTO)
        {
            var business = _mapper.Map<Business>(businessInsertDTO);
            var user = await _repositories.UserRepository.FindById(businessInsertDTO.UserId);
            business.User = user;
            business = await _repositories.BusinessRepository.AddOne(business);         
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<BusinessViewDTO>(business);

        }

        public async Task<BusinessViewDTO> Update(BusinessUpdateDTO businessUpdateDTO)
        {
            Business business = _mapper.Map<Business>(businessUpdateDTO);
            var data = await _repositories.BusinessRepository.UpdateOne(business, business.Id);
            if (data == null) throw new EntityNotFoundException("business");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<BusinessViewDTO>(data);
        }

        public async Task<BusinessViewDTO> UpdateImage(BusinessUpdateImageDTO updateImageDTO)
        {
            var data = await _repositories.BusinessRepository.UpdateImage(updateImageDTO.ImageStr, updateImageDTO.UserId);
            if (data == null) throw new EntityNotFoundException("business");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<BusinessViewDTO>(data);
        }
    }
}
