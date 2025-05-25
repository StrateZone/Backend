using AutoMapper;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;
using Microsoft.Extensions.DependencyInjection;
using StrateZone_Repository.Implements;

namespace StrateZone_Service.Implements
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IPriceService _priceService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IPriceService priceService, IMapper mapper, IServiceScopeFactory serviceScopeFactory)
        {
            _roomRepository = roomRepository;
            _priceService = priceService;
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<PagedList<RoomResponse>> GetRoomsAsync(RoomParameters parameters)
        {
            try
            {
                var result = await _roomRepository.GetRoomsAsync(parameters);
                
                var rooms = _mapper.Map<PagedList<RoomResponse>>(result);

                foreach (var r in rooms)
                {
                    if (r == null) continue;

                    string rt = r.Type;
                    PriceModel priceModel = await _priceService.GetPriceOfRoomTypeAsync(rt);

                    if (priceModel != null)
                    {
                        r.Price = priceModel.Price1;
                        r.Unit = priceModel.Unit;
                    }
                }

                return new PagedList<RoomResponse>(rooms, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<RoomResponse>> GetRoomsByRoomTypeAsync(RoomParameters parameters, string roomType)
        {
            try
            {
                var result = await _roomRepository.GetRoomsByTypeAsync(parameters, roomType);
                var rooms = _mapper.Map<PagedList<RoomResponse>>(result);

                foreach (var r in rooms)
                {
                    string rt = r.Type;
                    PriceModel priceModel = await _priceService.GetPriceOfRoomTypeAsync(rt);

                    if (priceModel != null)
                    {
                        r.Price = priceModel.Price1;
                        r.Unit = priceModel.Unit;
                    }
                }

                return new PagedList<RoomResponse>(rooms, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoomResponse> GetRoomByIdAsync(int id)
        {
            try
            {
                var result = await _roomRepository.GetRoomByIdAsync(id);

                PriceModel priceModel = await _priceService.GetPriceOfRoomTypeAsync(result.Type);
                var response = _mapper.Map<RoomResponse>(result);
                if (priceModel != null)
                {
                    response.Price = priceModel.Price1;
                    response.Unit = priceModel.Unit;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoomResponse> CreateRoomAsync(RoomRequest request)
        {
            try
            {
                RoomModel roomModel = new()
                {
                    RoomName = request.RoomName,
                    Capacity = request.Capacity,
                    Description = request.Description,
                    Type = request.Type,
                    Status = request.Status,
                    IsForMonthlyBooking = request.IsForMonthlyBooking
                };

                Room room = _mapper.Map<Room>(roomModel);
                var result = await _roomRepository.CreateRoomAsync(room);

                PriceModel priceModel = await _priceService.GetPriceOfRoomTypeAsync(result.Type);
                var response = _mapper.Map<RoomResponse>(result);
                if (priceModel != null)
                {
                    response.Price = priceModel.Price1;
                    response.Unit = priceModel.Unit;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoomResponse> UpdateRoomAsync(RoomModel roomModel, int id)
        {
            try
            {
                var room = _mapper.Map<Room>(roomModel);
                var result = await _roomRepository.UpdateRoomAsync(room, id);

                PriceModel priceModel = await _priceService.GetPriceOfRoomTypeAsync(result.Type);
                var response = _mapper.Map<RoomResponse>(result);
                if (priceModel != null)
                {
                    response.Price = priceModel.Price1;
                    response.Unit = priceModel.Unit;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoomResponse> DeleteRoomAsync(int id)
        {
            try
            {
                var result = await _roomRepository.DeleteRoomAsync(id);

                PriceModel priceModel = await _priceService.GetPriceOfRoomTypeAsync(result.Type);
                var response = _mapper.Map<RoomResponse>(result);
                if (priceModel != null)
                {
                    response.Price = priceModel.Price1;
                    response.Unit = priceModel.Unit;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> GetAllRoomtypesAsync()
        {
            try
            {
                return await _roomRepository.GetAllRoomtypesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> GetRegularRoomtypesAsync()
        {
            try
            {
                return await _roomRepository.GetRegularRoomtypesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> GetMonthlyRoomtypes()
        {
            try
            {
                return await _roomRepository.GetMonthlyRoomtypes();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoomModel> DisableRoomAsync(int id)
        {
            try
            {
                var result = await _roomRepository.GetRoomByIdAsync(id);

                if (result.Status == RoomStatus.unavailable) throw new Exception($"This room is already {result.Status}");

                result.Status = RoomStatus.unavailable;

                var updated = await _roomRepository.UpdateRoomAsync(result, id);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var tablesAppointmentService = scope.ServiceProvider.GetRequiredService<ITablesAppointmentService>();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var tablesAppointments = await tablesAppointmentService.GetAllActiveTablesAppointmentByRoomIdAsync(id);
                    foreach (var tablesAppointment in tablesAppointments)
                    {
                        var user = await userService.GetUserByAppointmentIdAsync((int)tablesAppointment.AppointmentId);
                        var invitedUser = await userService.GetAcceptedUserByTablesAppointmentIdAsync(tablesAppointment.Id);

                        if (invitedUser == null)
                            await tablesAppointmentService.ForceCancelTablesAppointmentDueToTableBecomesOFS(tablesAppointment.Id, user.UserId);
                        else
                            await tablesAppointmentService.ForceCancelTablesAppointmentDueToTableBecomesOFS(tablesAppointment.Id, user.UserId, invitedUser.UserId);
                    }
                });

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var tablesService = scope.ServiceProvider.GetRequiredService<ITableService>();
                    await tablesService.DisableTablesOnRoomAsync(id);
                });

                return _mapper.Map<RoomModel>(updated);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RoomModel> EnableRoomAsync(int id)
        {
            try
            {
                var result = await _roomRepository.GetRoomByIdAsync(id);

                if (result.Status == RoomStatus.available) throw new Exception($"This room is already {result.Status}");

                result.Status = RoomStatus.available;

                var updated = await _roomRepository.UpdateRoomAsync(result, id);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var tablesService = scope.ServiceProvider.GetRequiredService<ITableService>();
                    await tablesService.EnableTablesOnRoomAsync(id);
                });

                return _mapper.Map<RoomModel>(updated);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
