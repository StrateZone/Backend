using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public GameTypeService(IGameTypeRepository gameTypeRepository, IMapper mapper, IPriceRepository priceService, IServiceScopeFactory serviceScopeFactory)
        {
            _gameTypeRepository = gameTypeRepository;
            _mapper = mapper;
            _priceService = priceService;
            _serviceScopeFactory = serviceScopeFactory;
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

        public async Task<List<GameTypeModel>> GetActiveGameTypesAsync()
        {
            try
            {
                var result = await _gameTypeRepository.GetActiveGameTypesAsync();
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

        public async Task<GameTypeModel> UpdateAsync(GameTypeModel request, int id)
        {
            try
            {
                var mapped = _mapper.Map<GameType>(request);
                var result = await _gameTypeRepository.UpdateAsync(mapped, id);

                return _mapper.Map<GameTypeModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameTypeModel> DisableAsync(int id)
        {
            try
            {
                var toDisable = await GetGameTypeByIdAsync(id);

                if (toDisable.Status == "disabled") throw new Exception("This game type is already disabled");
            
                toDisable.Status = "disabled";
                var result = await UpdateAsync(toDisable, id);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var tablesAppointmentService = scope.ServiceProvider.GetRequiredService<ITablesAppointmentService>();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var tablesAppointments = await tablesAppointmentService.GetAllActiveTablesAppointmentByGameTypeIdAsync(id);
                    foreach (var tablesAppointment in tablesAppointments)
                    {
                        var user = await userService.GetUserByAppointmentIdAsync((int)tablesAppointment.AppointmentId);
                        await tablesAppointmentService.ForceCancelTablesAppointment(tablesAppointment.Id, user.UserId);
                    }
                });

                return result;
            }
            catch
            {
                throw ;
            }
        }

        public async Task<GameTypeModel> EnableAsync(int id)
        {
            try
            {
                var toEnable = await GetGameTypeByIdAsync(id);

                if (toEnable.Status == "active") throw new Exception("This game type is already active");

                toEnable.Status = "active";
                var result = await UpdateAsync(toEnable, id);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public Task<GameTypeModel> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
