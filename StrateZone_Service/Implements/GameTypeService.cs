using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
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
        private readonly IPriceRepository _priceService;
        private readonly IMapper _mapper;

        public GameTypeService(IGameTypeRepository gameTypeRepository, IMapper mapper, IPriceRepository priceService)
        {
            _gameTypeRepository = gameTypeRepository;
            _mapper = mapper;
            _priceService = priceService;
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

        public async Task<GameTypeModel> AddAsync(GameTypeRequest request)
        {
            try
            {
                var result = await _gameTypeRepository.AddAsync(new() { TypeName = request.TypeName });

                Price priceModel = new()
                { 
                    GameTypeId = result.TypeId,
                    Price1 = request.PricePerHour,
                    Unit = "per hour",
                    Type = "game_type",
                    MemberFee = false,
                    TeachingSalary = false,
                };
                await _priceService.CreatePriceAsync(priceModel);

                return _mapper.Map<GameTypeModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<GameTypeModel> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
