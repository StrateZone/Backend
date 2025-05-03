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
        Task<PagedList<Table>> GetTablesAsync(TablesAppointmentParameters parameters, string? search);
        Task<List<Table>> GetAvailableTablesAsync(DateTime StartTime, DateTime EndTime);
        Task<PagedList<Table>> GetTablesAsync(TableParameters parameters);
        Task<PagedList<Table>> GetTablesByGameTypeAsync(TableParameters parameters, string gameType);
        Task<PagedList<Table>> GetAvailableTablesAsync(TableParameters parameters);
        Task<PagedList<Table>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, string gameType);
        Task<PagedList<Table>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(TableParameters parameters, string[] gameTypes, string[] roomTypes);
        Task<Dictionary<string, List<Table>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount);
        Task<Table> UpdateTableAsync(Table table, int id);
    }
}