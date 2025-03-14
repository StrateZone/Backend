using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Implements
{
    public interface ITableService
    {
        Task<TableModel> CreateTableAsync(TableRequest request);
        Task<TableModel> DeleteTableAsync(int id);
        Task<PagedList<TableModel>> GetAvailableTablesAsync(TableParameters parameters);
        Task<PagedList<TableModel>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType);
        Task<TableModel> GetTableByIdAsync(int id);
        Task<PagedList<TableModel>> GetTablesAsync(TableParameters parameters);
        Task<PagedList<TableModel>> GetTablesByGameTypeAsync(TableParameters parameters, PostgreEnums.GameTypeEnum gameType);
        Task<TableModel> UpdateTableAsync(TableModel tableModel, int id);
    }
}