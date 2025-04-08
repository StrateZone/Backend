using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<NotificationModel> CreateNotificationAsync(NotificationRequest request)
        {
            try
            {
                NotificationModel notificationModel = new()
                {
                    ToUser = request.ToUser,
                    TablesAppointmentId = request.TablesAppointmentId,
                    TournamentId = request.TournamentId,
                    OrderId = request.OrderId,
                    Title = request.Title,
                    Content = request.Content,
                    Status = PostgreEnums.MessageStatus.unread,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)
                };

                var notification = _mapper.Map<Notification>(notificationModel);
                var result = await _notificationRepository.CreateNotificationAsync(notification);

                return _mapper.Map<NotificationModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<NotificationModel> DeleteAsync(int id)
        {
            try
            {
                var result = await _notificationRepository.DeleteAsync(id);

                return _mapper.Map<NotificationModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<NotificationModel> GetByIdAsync(int id)
        {
            try
            {
                var result = await _notificationRepository.GetByIdAsync(id);

                return _mapper.Map<NotificationModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<NotificationModel>> GetUserNotificationsAsync(int userId, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _notificationRepository.GetUserNotificationsAsync(userId, parameters);
                var mapped = _mapper.Map<PagedList<NotificationModel>>(result);

                return new PagedList<NotificationModel>(mapped, result.Count, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<NotificationModel> ReadByIdAsync(int id)
        {
            try
            {
                var result = await _notificationRepository.ReadByIdAsync(id);

                return _mapper.Map<NotificationModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<int> SendNotificationAboutExpiredOrRejectedAppointmentrequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NotificationModel> UpdateNotificationAsync(NotificationModel notification, int id)
        {
            throw new NotImplementedException();
        }
    }
}
