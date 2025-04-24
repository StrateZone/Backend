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

namespace StrateZone_Service.Implements
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly IPriceService _priceService;
        private readonly IMapper _mapper;

        public TableService(ITableRepository tableRepository, IPriceService priceService, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _priceService = priceService;
            _mapper = mapper;
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

        public async Task<PagedList<TableModel>> GetTablesByGameTypeAsync(TableParameters parameters, StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
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

        public async Task<PagedList<TableResponse>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
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

        public async Task<PagedList<TableResponse>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum[] gameTypes, PostgreEnums.RoomType[] roomTypes)
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

        public async Task<Dictionary<PostgreEnums.GameTypeEnum, List<TableResponse>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount)
        {
            try
            {
                var result = await _tableRepository.GetAvailableTablesForEachGameTypeInTimeRangeAsync(parameters, tableCount);
                var tables = _mapper.Map<Dictionary<GameTypeEnum, List<TableResponse>>>(result);

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

        public async Task<List<TableResponse>> GetAllTablesAsync()
        {
            try
            {
                var result = await _tableRepository.GetTablesAsync();
                return _mapper.Map<List<TableResponse>>(result);
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
    }
}
