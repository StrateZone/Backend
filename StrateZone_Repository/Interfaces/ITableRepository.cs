using StrateZone_Repository.Pagination;
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
        Task<Table> GetSimilarTableByIdAsync(DateTime startTime, DateTime endTime, int id);
        Task<PagedList<Table>> GetTablesAsync(TablesAppointmentParameters parameters, string? search);
        Task<List<Table>> GetAvailableTablesAsync(DateTime StartTime, DateTime EndTime);
        Task<List<Table>> GetTablesWithinASpecificTimeRangeInMonthAsync(List<(DateTime StartTime, DateTime EndTime)> times, string GameType, string RoomType);
        Task<PagedList<Table>> GetTablesAsync(TableParameters parameters);
        Task<PagedList<Table>> GetTablesByGameTypeAsync(TableParameters parameters, string gameType);
        Task<PagedList<Table>> GetAvailableTablesAsync(TableParameters parameters);
        Task<PagedList<Table>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, string gameType);
        Task<PagedList<Table>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(TableParameters parameters, string[] gameTypes, string[] roomTypes);
        Task<Dictionary<string, List<Table>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount);
        Task<Table> UpdateTableAsync(Table table, int id);
        Task EnableTablesOnRoomAsync(int id);
        Task DisableTablesOnRoomAsync(int id);
        Task EnableTablesOnGametypeAsync(int id);
        Task DisableTablesOnGametypeAsync(int id);
    }
}