using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<Notification> DeleteAsync(int id);
        Task<Notification> GetByIdAsync(int id);
        Task<Notification> ReadByIdAsync(int id);
        Task<PagedList<Notification>> GetUserNotificationsAsync(int userId, TablesAppointmentParameters parameters);
        Task<Notification> UpdateNotificationAsync(Notification notification, int id);
        Task<List<Notification>> ReadNotificationsAsync(int userId);
    }
}