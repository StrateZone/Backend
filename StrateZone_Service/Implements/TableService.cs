using AutoMapper;
using Azure.Core;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;
using static StrateZone_Repository.Parameters.PostgreEnums;
using Microsoft.Extensions.DependencyInjection;

namespace StrateZone_Service.Implements
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly IPriceService _priceService;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ScheduleTimeValidator _scheduleTimeValidator;

        public TableService(ITableRepository tableRepository, IPriceService priceService, IMapper mapper, IServiceScopeFactory serviceScopeFactory, ScheduleTimeValidator scheduleTimeValidator)
        {
            _tableRepository = tableRepository;
            _priceService = priceService;
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
            _scheduleTimeValidator = scheduleTimeValidator;
        }

        public async Task<PagedList<TableModel>> GetTablesAsync(TableParameters parameters)
        {
            try
            {
                var result = await _tableRepository.GetTablesAsync(parameters);
                var tables = _mapper.Map<PagedList<TableModel>>(result);

                return new PagedList<TableModel>(tables, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<TableResponse>> GetAvailableTablesAsync(TableParameters parameters)
        {
            try
            {
                var result = await _tableRepository.GetAvailableTablesAsync(parameters);
                var tables = _mapper.Map<PagedList<TableResponse>>(result);

                var gameTypesPrices = await _priceService.GetPricesPerHourEachGameTypeAsync();
                var roomTypesPrices = await _priceService.GetPricesPerHourEachRoomTypeAsync();

                decimal duration =
                    (decimal)(parameters.EndTime).Subtract(parameters.StartTime).TotalHours;

                foreach (var table in tables)
                {
                    table.StartDate = parameters.StartTime;
                    table.EndDate = parameters.EndTime;

                    var gtPrices = gameTypesPrices[(int)table.GameTypeId];
                    var rtPrice = roomTypesPrices[table.RoomType];

                    table.GameTypePrice = gtPrices;
                    table.RoomTypePrice = rtPrice;
                    table.DurationInHours = (float?)duration;
                    table.TotalPrice = (gtPrices + rtPrice) * duration;
                }

                return new PagedList<TableResponse>(tables, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<TableModel>> GetTablesByGameTypeAsync(TableParameters parameters, string gameType)
        {
            try
            {
                var result = await _tableRepository.GetTablesByGameTypeAsync(parameters, gameType);
                var tables = _mapper.Map<PagedList<TableModel>>(result);

                return new PagedList<TableModel>(tables, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<TableResponse>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, string gameType)
        {
            try
            {
                var result = await _tableRepository.GetAvailableTablesByGameTypeAsync(parameters, gameType);
                var tables = _mapper.Map<PagedList<TableResponse>>(result);

                var gameTypesPrices = await _priceService.GetPricesPerHourEachGameTypeAsync();
                var roomTypesPrices = await _priceService.GetPricesPerHourEachRoomTypeAsync();

                decimal duration =
                    (decimal)(parameters.EndTime).Subtract(parameters.StartTime).TotalHours;

                foreach (var table in tables)
                {
                    table.StartDate = parameters.StartTime;
                    table.EndDate = parameters.EndTime;

                    var gtPrices = gameTypesPrices[(int)table.GameTypeId];
                    var rtPrice = roomTypesPrices[table.RoomType];

                    table.GameTypePrice = gtPrices;
                    table.RoomTypePrice = rtPrice;
                    table.DurationInHours = (float?)duration;
                    table.TotalPrice = (gtPrices + rtPrice) * duration;
                }

                return new PagedList<TableResponse>(tables, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<TableResponse>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(TableParameters parameters, string[] gameTypes, string[] roomTypes)
        {
            try
            {
                var result = await _tableRepository.GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(parameters, gameTypes, roomTypes);
                var tables = _mapper.Map<PagedList<TableResponse>>(result);

                var gameTypesPrices = await _priceService.GetPricesPerHourEachGameTypeAsync();
                var roomTypesPrices = await _priceService.GetPricesPerHourEachRoomTypeAsync();

                decimal duration = 
                    (decimal)(parameters.EndTime).Subtract(parameters.StartTime).TotalHours;

                foreach (var table in tables)
                {
                    table.StartDate = parameters.StartTime;
                    table.EndDate = parameters.EndTime;

                    var gtPrices = gameTypesPrices[(int)table.GameTypeId];
                    var rtPrice = roomTypesPrices[table.RoomType];

                    table.GameTypePrice = gtPrices;
                    table.RoomTypePrice = rtPrice;
                    table.DurationInHours = (float?)duration;
                    table.TotalPrice = (gtPrices + rtPrice) * duration;
                }

                return new PagedList<TableResponse>(tables, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Dictionary<string, List<TableResponse>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount)
        {
            try
            {
                var result = await _tableRepository.GetAvailableTablesForEachGameTypeInTimeRangeAsync(parameters, tableCount);
                var tables = _mapper.Map<Dictionary<string, List<TableResponse>>>(result);

                foreach (var tableList in tables.Values)
                {
                    foreach (var table in tableList)
                    {
                        table.StartDate = parameters.StartTime;
                        table.EndDate = parameters.EndTime;

                        var prices = await _priceService.GetDetailedPriceOfTableFromTimeRangeAsync(table.TableId, parameters.StartTime, parameters.EndTime);

                        table.GameTypePrice = prices.ElementAt(0);
                        table.RoomTypePrice = prices.ElementAt(1);
                        table.DurationInHours = (float?)prices.ElementAt(2);
                        table.TotalPrice = prices.ElementAt(3);
                    }
                }

                return tables;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableResponse> GetTableByIdAsync(DateTime StartTime, DateTime EndTime, int id)
        {
            try
            {
                var result = await _tableRepository.GetTableByIdAsync(id);
                var table = _mapper.Map<TableResponse>(result);
            
                List<decimal> prices = await _priceService.GetDetailedPriceOfTableFromTimeRangeAsync(id, StartTime, EndTime);

                table.StartDate = StartTime;
                table.EndDate = EndTime;
                table.GameTypePrice = prices.ElementAt(0);
                table.RoomTypePrice = prices.ElementAt(1);
                table.DurationInHours = (float?)prices.ElementAt(2);
                table.TotalPrice = prices.ElementAt(3);

                return table;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableModel> CreateTableAsync(TableRequest request)
        {
            try
            {
                TableModel model = new()
                {
                    RoomId = request.Room_Id,
                    GameTypeId = request.GameType_Id
                };

                var toAdd = _mapper.Map<Table>(model);
                var result = await _tableRepository.CreateTableAsync(toAdd);

                return _mapper.Map<TableModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableModel> UpdateTableAsync(TableModel tableModel, int id)
        {
            try
            {
                var toAdd = _mapper.Map<Table>(tableModel);
                var result = await _tableRepository.UpdateTableAsync(toAdd, id);

                return _mapper.Map<TableModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableModel> DeleteTableAsync(int id)
        {
            try
            {
                var result = await _tableRepository.DeleteTableAsync(id);

                return _mapper.Map<TableModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<TableModel>> GetAllTablesAsync(TablesAppointmentParameters parameters, string? search)
        {
            try
            {
                var result = await _tableRepository.GetTablesAsync(parameters, search);
                var mapped = _mapper.Map<PagedList<TableModel>>(result);
                return new PagedList<TableModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TableResponse>> GetAllAvailableTablesAsync(DateTime StartTime, DateTime EndTime)
        {
            try
            {
                var result = await _tableRepository.GetAvailableTablesAsync(StartTime, EndTime);
                return _mapper.Map<List<TableResponse>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableResponse> DisableTableAsync(int id)
        {
            try
            {
                var result = await _tableRepository.GetTableByIdAsync(id);

                if (result.Status == TableStatus.out_of_service) throw new Exception($"This table is already {result.Status}");

                result.Status = TableStatus.out_of_service;
                result.GameType = null;
                result.Room = null;
                await _tableRepository.UpdateTableAsync(result, id);
                
                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var tablesAppointmentService = scope.ServiceProvider.GetRequiredService<ITablesAppointmentService>();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var tablesAppointments = await tablesAppointmentService.GetAllActiveTablesAppointmentByTableIdAsync(id);
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

                return _mapper.Map<TableResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableResponse> EnableTableAsync(int id)
        {
            try
            {
                var result = await _tableRepository.GetTableByIdAsync(id);

                if (result.Status == TableStatus.active) throw new Exception($"This table is already {result.Status}");

                result.Status = TableStatus.active;
                result.GameType = null;
                result.Room = null;
                await _tableRepository.UpdateTableAsync(result, id);

                return _mapper.Map<TableResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TableResponse> GetSimilarTableByIdAsync(DateTime StartTime, DateTime EndTime, int sampleId)
        {
            try
            {
                var result = await _tableRepository.GetSimilarTableByIdAsync(StartTime, EndTime, sampleId);
                var table = _mapper.Map<TableResponse>(result);

                List<decimal> prices = await _priceService.GetDetailedPriceOfTableFromTimeRangeAsync(result.TableId, StartTime, EndTime);

                table.StartDate = StartTime;
                table.EndDate = EndTime;
                table.GameTypePrice = prices.ElementAt(0);
                table.RoomTypePrice = prices.ElementAt(1);
                table.DurationInHours = (float?)prices.ElementAt(2);
                table.TotalPrice = prices.ElementAt(3);

                return table;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesMonthlyResponse> GetTablesWithinASpecificTimeRangeInMonthAsync(int Year, int Month, DayOfWeek dayOfWeek, TimeOnly StartTime, TimeOnly EndTime, string RoomType, string GameType)
        {
            try
            {
                List<(DateTime Start, DateTime End)> dates = new();

                int daysInMonth = DateTime.DaysInMonth(Year, Month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime date = new DateTime(Year, Month, day);

                    if (date.DayOfWeek != dayOfWeek) continue;

                    DateTime startDateTime = date.Add(StartTime.ToTimeSpan());
                    DateTime endDateTime = date.Add(EndTime.ToTimeSpan());

                    if (date < DateTime.UtcNow.AddHours(7)) continue;

                    dates.Add((startDateTime, endDateTime));
                }

                var result = await _tableRepository.GetTablesWithinASpecificTimeRangeInMonthAsync(dates, GameType, RoomType);
                var tables = _mapper.Map<List<TableResponse>>(result);

                Dictionary<DateOnly, TableResponse> response = new();
                for (int i = 0; i < tables.Count; ++i)
                {
                    var table = tables[i];
                    DateTime ScheduleTime = dates[i].Start;
                    DateTime tEndTime = dates[i].End;

                    var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(ScheduleTime, tEndTime, false);
                    if (!isValid)
                    {
                        response.Add(new DateOnly(ScheduleTime.Year, ScheduleTime.Month, ScheduleTime.Day), null);
                        continue;
                    }

                    List<decimal> prices = await _priceService.GetDetailedPriceOfTableFromTimeRangeAsync(table.TableId, ScheduleTime, tEndTime);

                    table.StartDate = ScheduleTime;
                    table.EndDate = tEndTime;
                    table.GameTypePrice = prices.ElementAt(0);
                    table.RoomTypePrice = prices.ElementAt(1);
                    table.DurationInHours = (float?)prices.ElementAt(2);
                    table.TotalPrice = prices.ElementAt(3);

                    response.Add(new DateOnly(ScheduleTime.Year, ScheduleTime.Month, ScheduleTime.Day), table);
                }

                return new()
                { 
                    DatesAndTables = response,
                    DayOfWeek = dayOfWeek.ToString(),

                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task EnableTablesOnRoomAsync(int id)
        {
            await _tableRepository.EnableTablesOnRoomAsync(id);
        }

        public async Task DisableTablesOnRoomAsync(int id)
        {
            await _tableRepository.DisableTablesOnRoomAsync(id);
        }

        public async Task EnableTablesOnGametypeAsync(int id)
        {
            await _tableRepository.EnableTablesOnGametypeAsync(id);
        }

        public async Task DisableTablesOnGametypeAsync(int id)
        {
            await _tableRepository.DisableTablesOnGametypeAsync(id);
        }
    }
}
