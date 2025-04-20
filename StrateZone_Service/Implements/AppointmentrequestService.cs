using AutoMapper;
using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class AppointmentrequestService : IAppointmentrequestService
    {
        private readonly IAppointmentrequestRepository _appointmentRequestRepository;
        private readonly ITablesAppointmentService _tablesAppointmentService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ScheduleTimeValidator _scheduleTimeValidator;
        private readonly INotificationService _notificationService;

        public AppointmentrequestService(IAppointmentrequestRepository appointmentRequestRepository, IMapper mapper, ITablesAppointmentService appointmentService, IPaymentService paymentService, ScheduleTimeValidator scheduleTimeValidator, INotificationService notificationService)
        {
            _appointmentRequestRepository = appointmentRequestRepository;
            _mapper = mapper;
            _tablesAppointmentService = appointmentService;
            _paymentService = paymentService;
            _scheduleTimeValidator = scheduleTimeValidator;
            _notificationService = notificationService;
        }

        public async Task<AppointmentrequestModel> CreateAppointmentRequestAsync(AppointmentrequestRequest request)
        {
            try
            {
                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(request.StartTime, request.EndTime, false);
                if (!isValid)
                {
                    throw new Exception(errorMessage);
                }

                if (request.ToUser == request.FromUser)
                    throw new Exception("Can not invite self.");

                DateTime currentTime = DateTime.UtcNow.AddHours(7);
                DateTime appointmentTime = request.StartTime;

                double timeUntilRequestExpiration = 
                    Math.Max(    
                        Math.Min(
                            24 * 2, 
                            appointmentTime.Subtract(currentTime).TotalHours * 0.5f
                            )
                        , 0.5f
                        );

                AppointmentrequestModel model = new()
                {
                    FromUser = request.FromUser,
                    ToUser = request.ToUser,
                    TableId = request.TableId,
                    AppointmentId = null,
                    TotalPrice = request.TotalPrice,
                    Status = PostgreEnums.RequestStatus.pending.ToString(),
                    StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Unspecified),
                    EndTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Unspecified),
                    ExpireAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified).AddHours(timeUntilRequestExpiration),
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                };

                var appointmentRequest = _mapper.Map<Appointmentrequest>(model);
                var result = await _appointmentRequestRepository.CreateAppointmentRequestAsync(appointmentRequest);

                NotificationRequest notification = new()
                {
                    ToUser = result.ToUser,
                    Title = $"Bạn có lời mời chơi cờ đến từ {result.FromUserNavigation.Username}!",
                    Content = $"{result.FromUserNavigation.Username} đã gửi cho bạn 1 lời mời chơi cờ vào lúc {request.StartTime.TimeOfDay}, " +
                    $"ngày {DateOnly.FromDateTime(request.StartTime)}. Bấm để xem tất cả lời mời của bạn.",
                    Type = NotificationType.appointment_request_from
                };
                await _notificationService.CreateNotificationAsync(notification);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> CreateAppointmentRequestsAsync(List<AppointmentrequestRequest> appointmentRequestModel)
        {
            try
            {
                if (appointmentRequestModel.Count <= 0) return null;

                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(appointmentRequestModel[0].StartTime, appointmentRequestModel[0].EndTime, false);
                if (!isValid)
                {
                    throw new Exception(errorMessage);
                }

                foreach (var request in appointmentRequestModel)
                {
                    if (request.ToUser == request.FromUser)
                        throw new Exception("Can not invite self.");
                }

                DateTime currentTime = DateTime.UtcNow.AddHours(7);
                DateTime appointmentTime = appointmentRequestModel[0].StartTime;

                double timeUntilRequestExpiration =
                    Math.Max(
                        Math.Min(
                            24 * 2,
                            appointmentTime.Subtract(currentTime).TotalHours * 0.5f
                            )
                        , 0.5f
                        );

                List<Appointmentrequest> mappedRequests = new();
                List<Notification> mappedNotifications = new();

                foreach (var request in appointmentRequestModel)
                {
                    AppointmentrequestModel model = new()
                    {
                        FromUser = request.FromUser,
                        ToUser = request.ToUser,
                        TableId = request.TableId,
                        AppointmentId = request.AppointmentId,
                        TotalPrice = request.TotalPrice,
                        Status = PostgreEnums.RequestStatus.pending.ToString(),
                        StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Unspecified),
                        EndTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Unspecified),
                        ExpireAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified).AddHours(timeUntilRequestExpiration),
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    };

                    mappedRequests.Add(_mapper.Map<Appointmentrequest>(model));
                }

                var result = await _appointmentRequestRepository.CreateAppointmentRequestsAsync(mappedRequests);

                List<NotificationRequest> notificationRequests = new();

                foreach (var request in result)
                {
                    NotificationRequest notification = new()
                    {
                        ToUser = request.ToUser,
                        Title = $"Bạn có lời mời chơi cờ đến từ {request.FromUserNavigation.Username}!",
                        Content = $"{request.FromUserNavigation.Username} đã gửi cho bạn 1 lời mời chơi cờ vào lúc {((DateTime)request.StartTime).TimeOfDay}, " +
                        $"ngày {DateOnly.FromDateTime((DateTime) request.StartTime)}. Bấm để xem tất cả lời mời của bạn.",
                        Type = NotificationType.appointment_request_from
                    };

                    notificationRequests.Add(notification);
                }

                await _notificationService.CreateNotificationsAsync(notificationRequests);

                return _mapper.Map<List<AppointmentrequestModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> CreateAdditionalAppointmentRequestsAsync(List<AppointmentrequestRequest> appointmentRequestModel)
        {
            try
            {
                if (appointmentRequestModel[0].StartTime <= DateTime.UtcNow.AddHours(7).AddHours(1.5f))
                    throw new Exception("Hiện đã sắp đến giờ chơi, không được phép mời thêm bạn.");

                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(appointmentRequestModel[0].StartTime, appointmentRequestModel[0].EndTime, false);
                if (!isValid)
                {
                    throw new Exception(errorMessage);
                }

                foreach (var request in appointmentRequestModel)
                {
                    if (request.ToUser == request.FromUser)
                        throw new Exception("Can not invite self.");
                }

                DateTime currentTime = DateTime.UtcNow.AddHours(7);
                DateTime appointmentTime = appointmentRequestModel[0].StartTime;

                double timeUntilRequestExpiration =
                    Math.Max(
                        Math.Min(
                            24 * 2,
                            appointmentTime.Subtract(currentTime).TotalHours * 0.5f
                            )
                        , 0.5f
                        );

                List<Appointmentrequest> mappedRequests = new();
                List<Notification> mappedNotifications = new();

                foreach (var request in appointmentRequestModel)
                {
                    AppointmentrequestModel model = new()
                    {
                        FromUser = request.FromUser,
                        ToUser = request.ToUser,
                        TableId = request.TableId,
                        AppointmentId = request.AppointmentId,
                        TotalPrice = request.TotalPrice,
                        Status = PostgreEnums.RequestStatus.pending.ToString(),
                        StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Unspecified),
                        EndTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Unspecified),
                        ExpireAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified).AddHours(timeUntilRequestExpiration),
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    };

                    mappedRequests.Add(_mapper.Map<Appointmentrequest>(model));
                }

                var result = await _appointmentRequestRepository.CreateAppointmentRequestsAsync(mappedRequests);

                List<NotificationRequest> notificationRequests = new();

                foreach (var request in result)
                {
                    NotificationRequest notification = new()
                    {
                        ToUser = request.ToUser,
                        Title = $"Bạn có lời mời chơi cờ đến từ {request.FromUserNavigation.Username}!",
                        Content = $"{request.FromUserNavigation.Username} đã gửi cho bạn 1 lời mời chơi cờ vào lúc {((DateTime)request.StartTime).TimeOfDay}, " +
                        $"ngày {DateOnly.FromDateTime((DateTime)request.StartTime)}. Bấm để xem tất cả lời mời của bạn.",
                        Type = NotificationType.appointment_request_from
                    };

                    notificationRequests.Add(notification);
                }

                await _notificationService.CreateNotificationsAsync(notificationRequests);

                return _mapper.Map<List<AppointmentrequestModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> DeleteAppointmentRequestAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.DeleteAppointmentRequestAsync(id);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> GetAppointmentRequestByIdAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestByIdAsync(id);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(int userId, int tablesAppointmentId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(userId, tablesAppointmentId);
                var appointmentRequestModels = _mapper.Map<List<AppointmentrequestModel>>(result);

                return appointmentRequestModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsFromUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsFromUserByUserIdAsync(parameters, userId);
                var appointmentRequestModels = _mapper.Map<PagedList<AppointmentrequestModel>>(result);

                foreach (var appointmentRequestModel in appointmentRequestModels)
                {
                    var ta = await _tablesAppointmentService.GetTablesAppointmentByTableIdAndAppointmentIdAsync(appointmentRequestModel.TableId, (int)appointmentRequestModel.AppointmentId);

                    appointmentRequestModel.TablesAppointmentStatus = ta.Status;
                    appointmentRequestModel.TablesAppointmentId = ta.Id;
                    appointmentRequestModel.TotalPrice = ta.Price;
                }

                return new PagedList<AppointmentrequestModel>(
                        appointmentRequestModels, result.TotalCount, result.CurrentPage, result.PageSize
                    );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentrequestModel>> GetAppointmentRequestsOfUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsOfUserByUserIdAsync(parameters, userId);
                var appointmentRequestModels = _mapper.Map<PagedList<AppointmentrequestModel>>(result);

                foreach (var appointmentRequestModel in appointmentRequestModels)
                {
                    var ta = await _tablesAppointmentService.GetTablesAppointmentByTableIdAndAppointmentIdAsync(appointmentRequestModel.TableId, (int)appointmentRequestModel.AppointmentId);

                    appointmentRequestModel.TablesAppointmentStatus = ta.Status;
                    appointmentRequestModel.TablesAppointmentId = ta.Id;
                    appointmentRequestModel.TotalPrice = ta.Price;
                }

                return new PagedList<AppointmentrequestModel>(appointmentRequestModels, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> GetAppointmentrequestsByAppointmentIdAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.GetAppointmentRequestsByAppointmentIdAsync(id);
                var appointmentRequestModels = _mapper.Map<List<AppointmentrequestModel>>(result);

                return appointmentRequestModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> GetCurrentAppointmentRequestsFromUserByUserAndTableIdAsync(int userId, int tableId, DateTime startTime, DateTime endTime)
        {
            try
            {
                startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Unspecified);
                endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Unspecified);

                var result = await _appointmentRequestRepository.GetCurrentAppointmentRequestsFromUserByUserAndTableAsync(userId, tableId, startTime, endTime);
                var appointmentRequestModels = _mapper.Map<List<AppointmentrequestModel>>(result);

                return appointmentRequestModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> UpdateAppointmentRequestAsync(AppointmentrequestModel appointmentRequestModel, int id)
        {
            try
            {
                var appointmentRequest = _mapper.Map<Appointmentrequest>(appointmentRequestModel);
                var result = await _appointmentRequestRepository.UpdateAppointmentRequestAsync(appointmentRequest, id);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> AcceptAppointmentrequestAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.AcceptAppointmentrequestAsync(id);

                NotificationRequest notification = new()
                {
                    ToUser = result.FromUser,
                    Title = "Lời mời đã được chấp nhận!",
                    Content = $"Lời mời tham gia chơi cờ của bạn gửi đến cho {result.ToUserNavigation.Username} đã được chấp nhận! Bấm để xem chi tiết.",
                    Type = NotificationType.appointment_request_to
                };
                await _notificationService.CreateNotificationAsync(notification);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentrequestModel> RejectAppointmentrequestAsync(int id)
        {
            try
            {
                var result = await _appointmentRequestRepository.RejectAppointmentrequestAsync(id);

                NotificationRequest notification = new()
                {
                    ToUser = result.FromUser,
                    Title = "Lời mời đã bị từ chối!",
                    Content = $"Lời mời tham gia chơi cờ của bạn gửi đến cho {result.ToUserNavigation.Username} đã bị đối phương từ chối! Bấm để xem chi tiết.",
                    Type = NotificationType.appointment_request_to
                };
                await _notificationService.CreateNotificationAsync(notification);

                return _mapper.Map<AppointmentrequestModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppointmentrequestModel>> CancelAllSentRequestFromUserAsync(int userId)
        {
            try
            {
                var result = await _appointmentRequestRepository.CancelAllSentRequestFromUserAsync(userId);
                return _mapper.Map<List<AppointmentrequestModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateExpiredAppointmentRequests()
        {
            try
            {
                return await _appointmentRequestRepository.UpdateExpiredAppointmentRequests();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetTablesAppointmentStatus(int id)
        {
            try
            {
                var ta = await _tablesAppointmentService.GetByIdAsync(id);
                
                var requests = (await _appointmentRequestRepository.GetAppointmentRequestsByTablesAppointmentIdAsync(ta.Id))
                                .Where(ar => ar.Status != RequestStatus.rejected && ar.Status != RequestStatus.rejected && ar.Status != RequestStatus.expired);

                if (requests == null || requests.Any()) return ta.Status;

                var acceptedRequest = requests.SingleOrDefault(r => r.Status == RequestStatus.accepted);
                
                if (acceptedRequest == null)
                {
                    return "awaiting_request_acceptance";
                }

                var payments = await _paymentService.GetPaymentsByTablesAppointmentIdAsync(ta.Id);
                var opponent_payment = payments.SingleOrDefault(p => p.UserId == acceptedRequest.ToUser);
                if (opponent_payment == null) return "awaiting_table_creation";
                else if (opponent_payment.PaymentStatus == PaymentStatus.unpaid.ToString()) return "awaiting_payment_from_opponent";
                return ta.Status;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<AppointmentrequestModel>> LinkAppointmentrequestsToAppointmentAsync(AppointmentModel appointmentModel)
        {
            try
            {
                AppointmentRequestParameters parameters = new()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue,
                };

                var tableIds = appointmentModel.TablesAppointments.Select(t => t.TableId).ToArray();

                var tablesAppointment = appointmentModel.TablesAppointments;

                var user_requests = (await _appointmentRequestRepository.GetAppointmentRequestsFromUserByUserIdAsync(parameters, appointmentModel.UserId))
                                .Where(ar => 
                                    tableIds.Contains(ar.TableId) 
                                    &&
                                    ar.AppointmentId == null
                                    &&
                                    (ar.Status == PostgreEnums.RequestStatus.pending || ar.Status == PostgreEnums.RequestStatus.accepted)
                                )
                                .ToList();

                foreach (var request in user_requests)
                {
                    request.AppointmentId = appointmentModel.AppointmentId;
                    await _appointmentRequestRepository.UpdateAppointmentRequestAsync(request, request.Id);
                }

                return _mapper.Map<List<AppointmentrequestModel>>(user_requests);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<AppointmentrequestModel> GetAppointmentrequestFromUserToUserInTableAsync(int fromUserId, int toUserId, int tableId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AppointmentrequestModel>> CancelAllAppointmentRequestsFromUserOnTableAsync(int userId, int tableId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var result = await _appointmentRequestRepository.CancelAllAppointmentRequestsFromUserOnTableAsync(userId, tableId, startTime, endTime);
                return _mapper.Map<List<AppointmentrequestModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
