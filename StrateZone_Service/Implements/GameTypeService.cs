using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class GameTypeService : IGameTypeService
    {
        private readonly IGameTypeRepository _gameTypeRepository;
        private readonly IGameExtensionService _gameExtensionService;
        private readonly IMapper _mapper;

        public GameTypeService(IGameTypeRepository gameTypeRepository, IGameExtensionService gameExtensionService, IMapper mapper)
        {
            _gameTypeRepository = gameTypeRepository;
            _gameExtensionService = gameExtensionService;
            _mapper = mapper;
        }

        public async Task<List<GameTypeModel>> GetGameTypesAsync()
        {
            try
            {
                var result = await _gameTypeRepository.GetGameTypesAsync();
                return _mapper.Map<List<GameTypeModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GameTypeModel>> GetGameTypesWithExtensionsAsync()
        {
            try
            {
                var result = await _gameTypeRepository.GetGameTypesWithExtensionsAsync();
                return _mapper.Map<List<GameTypeModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameTypeModel> GetGameTypeByIdAsync(int id)
        {
            try
            {
                var result = await _gameTypeRepository.GetGameTypesByIdAsync(id);
                return _mapper.Map<GameTypeModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameTypeModel> GetGameTypeWithExtensionsByIdAsync(int id)
        {
            try
            {
                var result = await _gameTypeRepository.GetGameTypeWithExtensionsByIdAsync(id);
                return _mapper.Map<GameTypeModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameTypeModel> GetGameTypeByGameExtensionIdAsync(int id)
        {
            try
            {
                var extension = await _gameExtensionService.GetGameExtensionByIdAsync(id);
                var result = await _gameTypeRepository.GetGameTypesByIdAsync((int) extension.TypeId) ?? throw new Exception("No game type for this game extension ID was found.");
                
                return _mapper.Map<GameTypeModel>(result); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
