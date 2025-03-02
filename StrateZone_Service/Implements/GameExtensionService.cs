using AutoMapper;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class GameExtensionService : IGameExtensionService
    {
        private readonly IGameExtensionRepository _gameExtensionRepository;
        private readonly IMapper _mapper;

        public GameExtensionService(IGameExtensionRepository gameTypeRepository, IMapper mapper)
        {
            _gameExtensionRepository = gameTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<GameExtensionModel>> GetGameExtensionsAsync()
        {
            try
            {
                var result = await _gameExtensionRepository.GetGameExtensionsAsync();
                return _mapper.Map<List<GameExtensionModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameExtensionModel> GetGameExtensionByIdAsync(int id)
        {
            try
            {
                var result = await _gameExtensionRepository.GetGameExtensionByIdAsync(id);
                return _mapper.Map<GameExtensionModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GameExtensionModel>> GetGameExtensionsByGameTypeIdAsync(int id)
        {
            try
            {
                var result = await _gameExtensionRepository.GetGameExtensionsByGameTypeIdAsync(id);
                return _mapper.Map<List<GameExtensionModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
