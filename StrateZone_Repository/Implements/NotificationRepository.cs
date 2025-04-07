using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly StrateZoneDbContext _context;

        public NotificationRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            try
            {
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                return notification;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Notification> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Notifications.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Notification>> GetUserNotificationsAsync(int userId, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = _context.Notifications
                                    .Where(n => n.ToUser == userId)
                                    .OrderByDescending(n => n.CreatedAt)
                                    .AsQueryable();

                return await PagedList<Notification>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Notification> UpdateNotificationAsync(Notification notification, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Notification> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
