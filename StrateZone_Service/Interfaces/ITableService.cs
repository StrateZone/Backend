using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Implements
{
    public interface ITableService
    {
        Task<TableModel> CreateTableAsync(TableRequest request);
        Task<TableModel> DeleteTableAsync(int id);
        Task<List<TableModel>> GetAvailableTablesAsync();
        Task<List<TableModel>> GetAvailableTablesByGameTypeAsync(PostgreEnums.GameTypeEnum gameType);
        Task<TableModel> GetTableByIdAsync(int id);
        Task<List<TableModel>> GetTablesAsync();
        Task<List<TableModel>> GetTablesByGameTypeAsync(PostgreEnums.GameTypeEnum gameType);
        Task<TableModel> UpdateTableAsync(TableModel tableModel, int id);
    }
}