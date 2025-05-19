using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Pagination;
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
        Task<PagedList<TableModel>> GetAllTablesAsync(TablesAppointmentParameters parameters, string? search);
        Task<List<TableResponse>> GetAllAvailableTablesAsync(DateTime StartTime, DateTime EndTime);
        Task<PagedList<TableResponse>> GetAvailableTablesByGameTypeAsync(TableParameters parameters, string gameType);
        Task<TableResponse> GetTableByIdAsync(DateTime StartTime, DateTime EndTime, int id);
        Task<TableResponse> GetSimilarTableByIdAsync(DateTime StartTime, DateTime EndTime, int sampleId);
        Task<PagedList<TableModel>> GetTablesAsync(TableParameters parameters);
        Task<PagedList<TableModel>> GetTablesByGameTypeAsync(TableParameters parameters, string gameType);
        Task<TableModel> UpdateTableAsync(TableModel tableModel, int id);
        Task<TableResponse> DisableTableAsync(int id);
        Task<TableResponse> EnableTableAsync(int id);
        Task<PagedList<TableResponse>> GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(TableParameters parameters, string[] gameTypes, string[] roomTypes);
        Task<Dictionary<string, List<TableResponse>>> GetAvailableTablesForEachGameTypeInTimeRangeAsync(TableParameters parameters, int tableCount);
        Task EnableTablesOnRoomAsync(int id);
        Task DisableTablesOnRoomAsync(int id);
        Task EnableTablesOnGametypeAsync(int id);
        Task DisableTablesOnGametypeAsync(int id);
    }
}