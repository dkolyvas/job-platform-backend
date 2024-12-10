using AutoMapper;
using JobPlatform.DTO.SubscriptionType;
using JobPlatform.Repositories;
using JobPlatform.Exceptions;
using JobPlatform.Data;

namespace JobPlatform.Services
{
    public class SubscriptionTypeService: ISubscriptionTypeService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;

        public SubscriptionTypeService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        public async Task<SubscriptionTypeViewDTO> AddOne(SubscriptionTypeInsertDTO insertDTO)
        {
            SubscriptionType type = _mapper.Map<SubscriptionType>(insertDTO);   
            type = await _repositories.SubscriptionTypesRepository.AddOne(type);
            if(! await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SubscriptionTypeViewDTO>(type);
        }

        public async Task<bool> DeleteById(int id)
        {
            bool result = await _repositories.SubscriptionTypesRepository.Delete(id);
            if (!result) throw new EntityNotFoundException("subscription type");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<IEnumerable<SubscriptionTypeViewDTO>> GetAll()
        {
            var data = await _repositories.SubscriptionTypesRepository.FindAll();
            return _mapper.Map<IEnumerable<SubscriptionTypeViewDTO>>(data);
        }

        public async Task<SubscriptionTypeViewDTO> GetById(int id)
        {
            var data = await _repositories.SubscriptionTypesRepository.FindById(id);
            if (data == null) throw new EntityNotFoundException("subscription type");
            return _mapper.Map<SubscriptionTypeViewDTO>(data);
        }

        public async Task<SubscriptionTypeViewDTO> UpdateOne(SubscriptionTypeUpdateDTO updateDTO)
        {
            SubscriptionType?  type = _mapper.Map<SubscriptionType>(updateDTO);
            type  = await _repositories.SubscriptionTypesRepository.UpdateOne(type);
            if (type is null) throw new EntityNotFoundException("subscription type");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SubscriptionTypeViewDTO>(type);
        }
    }
}
