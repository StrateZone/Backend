using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface ITableRepository
    {
        Task<Table> CreateTableAsync(Table table);
        Task<Table> DeleteTableAsync(int id);
        Task<Table> GetTableByIdAsync(int id);
        Task<List<Table>> GetTablesAsync();
        Task<List<Table>> GetTablesByGameTypeAsync(PostgreEnums.GameTypeEnum gameType);
        Task<List<Table>> GetAvailableTablesAsync();
        Task<List<Table>> GetAvailableTablesByGameTypeAsync(PostgreEnums.GameTypeEnum gameType);
        Task<Table> UpdateTableAsync(Table table, int id);
    }
}