using StrateZone_Repository.Entities;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IPointsHistoryRepository
    {
        Task<PointsHistory> AddAsync(PointsHistory history);
        Task DeleteAsync(int id);
        Task<PagedList<PointsHistory>> GetAllAsync(TablesAppointmentParameters parameters);
        Task<PointsHistory> GetByIdAsync(int id);
        Task<PagedList<PointsHistory>> GetByUserIdAsync(int userId, TablesAppointmentParameters parameters);
        Task UpdateAsync(PointsHistory history, int id);
    }
}