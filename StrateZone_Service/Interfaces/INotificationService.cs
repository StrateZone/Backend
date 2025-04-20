using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationModel> CreateNotificationAsync(NotificationRequest notification);
        Task<List<NotificationModel>> CreateNotificationsAsync(List<NotificationRequest> notification);
        Task<List<NotificationModel>> CreateNotificationsForRejectedTablesAppoimentsAsync(List<NotificationRequest> notification);
        Task<NotificationModel> DeleteAsync(int id);
        Task<NotificationModel> GetByIdAsync(int id);
        Task<NotificationModel> ReadByIdAsync(int id);
        Task<PagedList<NotificationModel>> GetUserNotificationsAsync(int userId, TablesAppointmentParameters parameters);
        Task<NotificationModel> UpdateNotificationAsync(NotificationModel notification, int id);
        Task<List<NotificationModel>> ReadNotificationsAsync(int userId);
        Task<int> SendNotificationAboutExpiredOrRejectedAppointmentrequestsAsync();

    }
}
