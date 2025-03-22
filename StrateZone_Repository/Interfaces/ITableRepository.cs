using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface ITableRepository
    {
        Task<Table> CreateTableAsync(Table table);
        Task<Table> DeleteTableAsync(int id);
        Task<Table> GetTableByIdAsync(int id);
        Task<PagedList<Table>> GetTablesAsync(TableParameters parameters);
        Task<PagedList<Table>> GetTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType);
        Task<PagedList<Table>> GetAvailableTablesAsync(TableParameters parameters);
        Task<PagedList<Table>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType);
        Task<PagedList<Table>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(TableParameters parameters, GameTypeEnum[] gameTypes, RoomType[] roomTypes);
        Task<Dictionary<GameTypeEnum, List<Table>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount);
        Task<Table> UpdateTableAsync(Table table, int id);
    }
}