using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface IPointsHistoryService
    {
        Task<PointsHistoryModel> AddAsync(PointsHistoryModel model);
        Task DeleteAsync(int id);
        Task<PagedList<PointsHistoryModel>> GetAllAsync(TablesAppointmentParameters parameters);
        Task<PointsHistoryModel> GetByIdAsync(int id);
        Task<PagedList<PointsHistoryModel>> GetByUserIdAsync(int userId, TablesAppointmentParameters parameters);
        Task UpdateAsync(PointsHistoryModel model, int id);
    }
}