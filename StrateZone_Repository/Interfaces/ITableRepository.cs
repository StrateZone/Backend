using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

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
        Task<Table> UpdateTableAsync(Table table, int id);
    }
}