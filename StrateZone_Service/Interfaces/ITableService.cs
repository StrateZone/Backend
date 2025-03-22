using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public interface ITableService
    {
        Task<TableModel> CreateTableAsync(TableRequest request);
        Task<TableModel> DeleteTableAsync(int id);
        Task<PagedList<TableResponse>> GetAvailableTablesAsync(TableParameters parameters);
        Task<PagedList<TableResponse>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType);
        Task<TableModel> GetTableByIdAsync(int id);
        Task<PagedList<TableModel>> GetTablesAsync(TableParameters parameters);
        Task<PagedList<TableModel>> GetTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType);
        Task<TableModel> UpdateTableAsync(TableModel tableModel, int id);
        Task<PagedList<TableResponse>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(TableParameters parameters, GameTypeEnum[] gameTypes, RoomType[] roomTypes);
        Task<Dictionary<GameTypeEnum, List<TableResponse>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount);
    }
}