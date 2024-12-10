using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.Region;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;

        public RegionService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        public async Task<RegionViewDTO> AddOne(RegionInsertDTO insertDTO)
        {
            Region region = _mapper.Map<Region>(insertDTO);
            region = await _repositories.RegionsRepository.AddOne(region);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<RegionViewDTO>(region);
        }

        public async Task<bool> DeleteById(int id)
        {
            bool result = await _repositories.RegionsRepository.Delete(id);
            if (!result) throw new EntityNotFoundException("region");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<IEnumerable<RegionViewDTO>> GetAll()
        {
            var data = await _repositories.RegionsRepository.FindAll();
            return _mapper.Map<IEnumerable<RegionViewDTO>>(data);
        }

        public async Task<RegionViewDTO> GetById(int id)
        {
            var data = await _repositories.RegionsRepository.FindById(id);
            if (data is null) throw new EntityNotFoundException("region");
            return _mapper.Map<RegionViewDTO>(data);
        }

        public async Task<RegionViewDTO> UpdateOne(RegionUpdateDTO updateDTO)
        {
            Region? region = _mapper.Map<Region>(updateDTO);
            region = await _repositories.RegionsRepository.UpdateOne(region);
            if (region is null) throw new EntityNotFoundException("region");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<RegionViewDTO>(region);
        }
    }
}
