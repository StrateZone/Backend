using AutoMapper;
using StrateZone_Repository.Pagination;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System.Net.Http;
using System.Text;
using static StrateZone_Repository.Parameters.PostgreEnums;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using QRCoder;
using Org.BouncyCastle.Ocsp;

namespace StrateZone_Service.Implements
{
    public class TablesAppointmentService : ITablesAppointmentService
    {
        private readonly IAppointmentrequestRepository _requestRepository;
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly IPriceService _priceService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userService;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly ISystemService _systemService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TablesAppointmentService(ITablesAppointmentRepository tablesAppointmentRepository, IMapper mapper, IPriceService priceService, IPaymentService paymentService, IWalletService walletService, ITransactionService transactionService, IAppointmentrequestRepository repository, INotificationService notificationService, IUserRepository userService, HttpClient httpClient, IServiceScopeFactory serviceProvider, ISystemService systemService)
        {
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _mapper = mapper;
            _priceService = priceService;
            _paymentService = paymentService;
            _walletService = walletService;
            _transactionService = transactionService;
            _requestRepository = repository;
            _notificationService = notificationService;
            _userService = userService;
            _httpClient = httpClient;
            _serviceScopeFactory = serviceProvider;
            _systemService = systemService;
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsAsync(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentAsync(parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);
            
                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentByTableIdAsync(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentByTableIdAsync(id, parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);

                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentResponse>> GetAllTablesAppointmentByAppointmentIdAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentByAppointmentIdAsync(id);
                return _mapper.Map<List<TablesAppointmentResponse>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentResponse> GetTablesAppointmentByTableIdAndAppointmentIdAsync(int tableId, int appointmentId)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetTablesAppointmentByTableIdAndAppointmentIdAsync(tableId, appointmentId);
                return _mapper.Map<TablesAppointmentResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> CreateTablesAppointmentAsync(TablesAppointmentModel tablesAppointmentModel)
        {
            try
            {
                tablesAppointmentModel.Price = await _priceService.GetPriceOfTablesAppointmentAsync(tablesAppointmentModel);
                var tablesAppointment = _mapper.Map<TablesAppointment>(tablesAppointmentModel);

                var result = await _tablesAppointmentRepository.CreateTablesAppointmentAsync(tablesAppointment);
                return _mapper.Map<TablesAppointmentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentModel>> CreateTablesAppointmentsFromAppointmentAsync(AppointmentModel appointmentModel)
        {
            try
            {
                var appointment = _mapper.Map<Appointment>(appointmentModel);
                var result = await _tablesAppointmentRepository.CreateTablesAppointmentsFromAppointmentAsync(appointment);

                var mappedResult = _mapper.Map<List<TablesAppointmentModel>>(result);

                return mappedResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> DeleteTablesAppointmentAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.DeleteTablesAppointmentAsync(id);
                return _mapper.Map<TablesAppointmentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentResponse> GetByIdAsync(int id)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetByIdAsync(id);
                return _mapper.Map<TablesAppointmentResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> CheckInTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var tablesAppointmentResponse = await GetByIdAsync(tablesAppointmentId);
                var tablesAppointment = _mapper.Map<TablesAppointmentModel>(tablesAppointmentResponse);

                var payment = (await _paymentService.GetPaymentsByTablesAppointmentIdAsync(tablesAppointmentId))
                            .SingleOrDefault(p => p.UserId == userId) 
                            ?? throw new Exception("No payment was found for this tables appointment.");

                if ((PaymentStatus) Enum.Parse(typeof(PaymentStatus), payment.PaymentStatus) == PaymentStatus.unpaid)
                    throw new Exception($"Check-in failed: Unpaid appointment. Please proceed with the payment first!");

                string errorMessage = (AppointmentStatus) Enum.Parse(typeof(AppointmentStatus), tablesAppointment.Status) switch
                {
                    AppointmentStatus.pending => "This appointment hasn't been confirmed.",
                    AppointmentStatus.refunded => "This appointment has already been cancelled and refunded.",
                    AppointmentStatus.checked_in => "This appointment has already been checked-in.",
                    AppointmentStatus.cancelled => "This appointment has been cancelled.",
                    AppointmentStatus.expired => "This appointment is expired.",
                    AppointmentStatus.completed => "This appointment is already completed.",
                    _ => string.Empty,
                };

                if (!string.IsNullOrEmpty(errorMessage)) throw new Exception($"Check-in failed: {errorMessage}");

                int minutes_beforeCheckin = await _systemService.GetAppointmentCheckinTimeInMinuesAsync(1);

                if (DateTime.UtcNow.AddHours(7) < tablesAppointment.ScheduleTime.AddMinutes(-minutes_beforeCheckin))
                    throw new Exception($"Check-in is not yet opened: Check-in only available {minutes_beforeCheckin} minutes prior to schedule time!");

                tablesAppointment.Status = AppointmentStatus.checked_in.ToString();

                var result = await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);

                var userCheckin = await _userService.GetUserByIdAsync(userId);
                
                int pointsCalculate = await _systemService.GetUserPointsForCheckingInByTablesPrice((decimal)tablesAppointment.Price, 1);

                userCheckin.Points += pointsCalculate;
                await _userService.UpdateUserAsync(userCheckin, userId);

                NotificationRequest notificationRequest = new()
                {
                    ToUser = userId,
                    Title = $"Check-in cho bàn số {tablesAppointment.TableId} thành công!",
                    Content = $"Check-in hoàn tất! Bạn được cộng {pointsCalculate} điểm cá nhân, điểm khi tích đủ có thể dùng để đổi sang vouchers giảm giá cho lần đặt hẹn kế tiếp. " +
                    $"Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!",
                    TablesAppointmentId = tablesAppointmentId,
                    Type = NotificationType.tables_appointment,
                };
                await _notificationService.CreateNotificationAsync(notificationRequest);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        
        public async Task<TablesAppointmentModel> CheckoutTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var tablesAppointmentResponse = await GetByIdAsync(tablesAppointmentId);
                var tablesAppointment = _mapper.Map<TablesAppointmentModel>(tablesAppointmentResponse);

                if (tablesAppointment.Status != AppointmentStatus.checked_in.ToString()) 
                    throw new Exception($"Check-out failed: This table appointment is not checked-in.");

                tablesAppointment.Status = AppointmentStatus.completed.ToString();
                var result = await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
                
                var scope = _serviceScopeFactory.CreateScope();
                var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

                await appointmentService.UpdateStatusForAppointmentBasedOnTablesAppointments();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentModel> CancelTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var now = DateTime.UtcNow.AddHours(7);
                var refundCalculation = await CalculateRefundAmountOnAppointmentCancellation(userId, tablesAppointmentId, now);
                var tablesAppointment = refundCalculation.TablesAppointmentModel;

                var status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), tablesAppointment.Status);
                if (status is AppointmentStatus.cancelled or AppointmentStatus.refunded or AppointmentStatus.checked_in or AppointmentStatus.expired or AppointmentStatus.completed or AppointmentStatus.incoming)
                {
                    string errorMessage = status switch
                    {
                        AppointmentStatus.cancelled => "This appointment has already been cancelled.",
                        AppointmentStatus.refunded => "This appointment has already been cancelled and refunded.",
                        AppointmentStatus.checked_in => "This appointment has already been checked-in.",
                        AppointmentStatus.expired => "This appointment is expired.",
                        AppointmentStatus.completed => "This appointment is already completed.",
                        AppointmentStatus.incoming => "Cannot cancel incoming appointments.",
                        _ => "Invalid appointment status."
                    };
                    throw new Exception($"Cancellation failed: {errorMessage}");
                }

                if (refundCalculation.RefundStatus == RefundStatus.cancellation_fail)
                    throw new Exception(refundCalculation.Message);

                if (refundCalculation.RefundStatus == RefundStatus.no_refund_while_refund_for_invited_user)
                {
                    await HandleInvitedUserRefund(tablesAppointment, userId, (int) refundCalculation.InvitedUserId);
                }
                else
                {
                    await HandleUserRefund(tablesAppointment, userId, refundCalculation.RefundAmount);
                }

                tablesAppointment.Status = AppointmentStatus.cancelled.ToString();
                await CancelAppointmentRequests(tablesAppointmentId);

                return await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task HandleUserRefund(TablesAppointmentModel appointment, int userId, decimal refundAmount)
        {
            try
            {
                if (refundAmount > 0)
                {
                    await _walletService.DepositWalletByUserIdAsync((int)refundAmount, userId);

                    var transaction = new TransactionModel
                    {
                        Amount = refundAmount,
                        Content = $"Hoàn tiền {refundAmount} VND cho đơn đặt ở bàn số {appointment.TableId}, đơn #{appointment.AppointmentId}.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        OfUser = userId,
                        TransactionType = TransactionType.refund,
                    };
                    _ = Task.Run(() => _transactionService.SaveTransaction(transaction));

                    var notification = new NotificationRequest
                    {
                        ToUser = userId,
                        Title = "Hủy đơn đặt bàn thành công!",
                        Content = $"Bạn đã hủy đơn đặt ở bàn số {appointment.TableId}, đơn #{appointment.AppointmentId}. {refundAmount} VND đã được hoàn về ví của bạn!",
                        Type = NotificationType.appointment
                    };
                    _ = Task.Run(() => _notificationService.CreateNotificationAsync(notification));
                }
                else
                {
                    var notification = new NotificationRequest
                    {
                        ToUser = userId,
                        Title = "Hủy đơn đặt bàn thành công!",
                        Content = $"Bạn đã hủy đơn đặt ở bàn số {appointment.TableId}, đơn #{appointment.AppointmentId}.",
                        Type = NotificationType.appointment
                    };
                    _ = Task.Run(() => _notificationService.CreateNotificationAsync(notification));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task HandleInvitedUserRefund(TablesAppointmentModel appointment, int cancellingUserId, int invitedUserId)
        {
            try
            {
                await _walletService.DepositWalletByUserIdAsync((int)appointment.Price, invitedUserId);

                var transaction = new TransactionModel
                {
                    Amount = appointment.Price,
                    Content = $"Hoàn tiền {appointment.Price} VND cho đơn được mời tham gia ở bàn số {appointment.TableId}, đơn #{appointment.AppointmentId}.",
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    OfUser = invitedUserId,
                    TransactionType = TransactionType.refund,
                };
                _ = Task.Run(() => _transactionService.SaveTransaction(transaction));

                var cancellingUser = await _userService.GetUserByIdAsync(cancellingUserId);

                var notificationToCancellingUser = new NotificationRequest
                {
                    ToUser = cancellingUserId,
                    Title = "Hủy đơn đặt bàn thành công!",
                    Content = $"Bạn đã hủy đơn đặt ở bàn số {appointment.TableId}, đơn #{appointment.AppointmentId}. " +
                              "Do đây là đơn đặt có sự tham gia của người chơi khác, bạn sẽ không được hoàn tiền!",
                    Type = NotificationType.appointment_request_from
                };

                var notificationToInvitedUser = new NotificationRequest
                {
                    ToUser = invitedUserId,
                    Title = $"{cancellingUser.Username} đã hủy đơn đặt bàn!",
                    Content = $"{cancellingUser.Username} đã hủy đơn đặt bàn mà bạn đã chấp nhận tham gia trước đó. {appointment.Price} VND đã tự động được hoàn về ví của bạn!",
                    Type = NotificationType.appointment_request_from
                };

                _ = Task.Run(() => _notificationService.CreateNotificationsAsync(new() { notificationToCancellingUser, notificationToInvitedUser }));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task CancelAppointmentRequests(int tablesAppointmentId)
        {
            var requests = await _requestRepository.GetAppointmentRequestsByTablesAppointmentIdAsync(tablesAppointmentId);
            foreach (var request in requests)
            {
                if (request.Status == RequestStatus.pending) request.Status = RequestStatus.cancelled;
                else if (request.Status == RequestStatus.accepted) request.Status = RequestStatus.table_cancelled;
            }
            _ = Task.Run(() => _requestRepository.MassUpdateAppointmentRequestsAsync(requests));
        }

        public async Task<TablesAppointmentModel> ForceCancelTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var refundCalculation = await CalculateRefundAmountOnAppointmentCancellation(userId, tablesAppointmentId, DateTime.UtcNow.AddHours(7));

                var tablesAppointment = refundCalculation.TablesAppointmentModel;
                string errorMessage = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), tablesAppointment.Status) switch
                {
                    AppointmentStatus.cancelled => "This appointment has already been cancelled.",
                    AppointmentStatus.refunded => "This appointment has already been cancelled and refunded.",
                    AppointmentStatus.checked_in => "This appointment has already been checked-in.",
                    AppointmentStatus.expired => "This appointment is expired.",
                    AppointmentStatus.completed => "This appointment is already completed.",
                    _ => string.Empty,
                };

                if (!string.IsNullOrEmpty(errorMessage)) throw new Exception($"Cancellation failed: {errorMessage}");

                if (refundCalculation.RefundStatus == RefundStatus.no_refund || refundCalculation.RefundStatus == RefundStatus.cancellation_fail)
                {
                    tablesAppointment.Status = AppointmentStatus.cancelled.ToString();
                    return await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
                }

                if (refundCalculation.RefundStatus != RefundStatus.no_refund_while_refund_for_invited_user)
                {
                    var refundAmount = refundCalculation.RefundAmount;
                    await _walletService.DepositWalletByUserIdAsync((int)refundAmount, userId);

                    var newTransaction = new TransactionModel
                    {
                        Amount = refundAmount,
                        Content =
                            $"Hoàn tiền {refundAmount} VND cho đơn đặt ở bàn số {tablesAppointment.TableId}, " +
                            $"đơn #{tablesAppointment.AppointmentId}.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        OfUser = userId,
                        TransactionType = TransactionType.refund,
                    };
                    _ = Task.Run(() => _transactionService.SaveTransaction(newTransaction));
                }
                else 
                {
                    var refundAmount = refundCalculation.RefundAmount;

                    await _walletService.DepositWalletByUserIdAsync((int)tablesAppointment.Price, (int)refundCalculation.InvitedUserId);

                    var newTransaction = new TransactionModel
                    {
                        Amount = tablesAppointment.Price,
                        Content =
                            $"Hoàn tiền {tablesAppointment.Price} VND cho đơn được mời tham gia ở bàn số {tablesAppointment.TableId}, " +
                            $"đơn #{tablesAppointment.AppointmentId}.",
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        OfUser = refundCalculation.InvitedUserId,
                        TransactionType = TransactionType.refund,
                    };
                    _ = Task.Run(() => _transactionService.SaveTransaction(newTransaction));
                }

                tablesAppointment.Status = AppointmentStatus.cancelled.ToString();

                var requests = await _requestRepository.GetAppointmentRequestsByTablesAppointmentIdAsync(tablesAppointmentId);
                foreach (var req in requests)
                {
                    req.Status = RequestStatus.cancelled;
                   
                }
                _ = Task.Run(() => _requestRepository.MassUpdateAppointmentRequestsAsync(requests));

                return await UpdateTablesAppointmentAsync(tablesAppointment, tablesAppointmentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentRefundResponse> CalculateRefundAmountOnAppointmentCancellation(int userId, int tablesAppointmentId, DateTime CancelTime)
        {
            var tablesAppointment = await _tablesAppointmentRepository.GetByIdAsync(tablesAppointmentId)
                                ?? throw new Exception("Tables appointment with this ID does not exist.");

            if (CancelTime < tablesAppointment.CreatedAt) throw new Exception("Cancel time must the later than created_time.");

            var model = _mapper.Map<TablesAppointmentModel>(tablesAppointment);

            var cancelledTablesAppointmentsWithinThisWeek = await _tablesAppointmentRepository.GetNumberOfTablesAppointmentCancelledByUserInAWeekSpanAsync(userId, CancelTime);

            DateTime ScheduleTime = tablesAppointment.ScheduleTime;
            DateTime CreatedTime = (DateTime)tablesAppointment.CreatedAt;

            decimal refund100_hours = await _systemService.GetAppointmentRefund100TimeInHoursAsync(1),
                    incoming_hours = await _systemService.GetAppointmentIncomingTimeInHoursAsync(1);

            DateTime TimeGate_BlockAppointmentCancellation = ScheduleTime.AddHours((double)(incoming_hours * -1)),
                     TimeGate_Refund50_OnCancellation = ScheduleTime.AddHours((double)(refund100_hours * -1));

            var bookingPayments = await _paymentService.GetPaymentsByTablesAppointmentIdAsync(tablesAppointmentId);
            var paymentForUser = bookingPayments.SingleOrDefault(p => p.UserId == userId);
            if (paymentForUser == null || paymentForUser.PaymentStatus == PaymentStatus.unpaid.ToString())
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = 0,
                    RefundStatus = RefundStatus.no_refund,
                    Message = "Không được hoàn tiền. Lí do: Đơn đặt bàn này chưa thanh toán.",
                    CancelUserId = bookingPayments.FirstOrDefault(p => p.UserId == userId)?.UserId,
                    InvitedUserId = bookingPayments.FirstOrDefault(p => p.UserId != userId)?.UserId,
                    NumerOfTablesCancelledThisWeek = cancelledTablesAppointmentsWithinThisWeek,
                    CancellationTime = CancelTime,
                    Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                };
            }

            if (bookingPayments.Any(p => p.UserId != userId && p.PaymentStatus == PaymentStatus.paid.ToString()))
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = 0,
                    RefundStatus = RefundStatus.no_refund_while_refund_for_invited_user,
                    Message = "Không được hoàn tiền. Lí do: Đơn đặt bàn có mời người chơi khác (người bạn đã mời vẫn " +
                    "sẽ được hoàn tiền).",
                    CancelUserId = bookingPayments.FirstOrDefault(p => p.UserId == userId)?.UserId,
                    InvitedUserId = bookingPayments.FirstOrDefault(p => p.UserId != userId)?.UserId,
                    NumerOfTablesCancelledThisWeek = cancelledTablesAppointmentsWithinThisWeek,
                    CancellationTime = CancelTime,
                    Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                };
            }

            if (CancelTime >= TimeGate_BlockAppointmentCancellation)
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = 0,
                    RefundStatus = RefundStatus.cancellation_fail,
                    Message = $"Không được phép hủy đơn trong vòng {incoming_hours} tiếng trước giờ hẹn.",
                };
            }

            int maxTablesCancelPerWeek = await _systemService.GetMaxNumberOfTablesCancelPerWeek(1);

            if (cancelledTablesAppointmentsWithinThisWeek > maxTablesCancelPerWeek)
            {
                DateOnly monday = DateOnly.FromDateTime(
                        CancelTime.AddDays(-(int)CancelTime.DayOfWeek + (CancelTime.DayOfWeek == DayOfWeek.Sunday ? -6 : 1))
                    );
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = 0,
                    RefundStatus = RefundStatus.no_refund,
                    Message = $"Không được hoàn tiền. " +
                    $"Lí do: Bạn đã hủy nhiều hơn {maxTablesCancelPerWeek} bàn trong tuần này, tính từ ngày {monday:dd/MM/yyyy} (thứ hai) " +
                    $"đến {DateOnly.FromDateTime(CancelTime):dd/MM/yyyy} (hiện tại).",
                    NumerOfTablesCancelledThisWeek = cancelledTablesAppointmentsWithinThisWeek,
                    CancellationTime = CancelTime,
                    Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                    CancelUserId = bookingPayments.FirstOrDefault(p => p.UserId == userId)?.UserId,
                    InvitedUserId = bookingPayments.FirstOrDefault(p => p.UserId != userId)?.UserId,
                };
            }

            if (CreatedTime >= TimeGate_Refund50_OnCancellation)
            {
                double TimeDiff_CreatedTimeUntilCancellationBlockTime = TimeGate_BlockAppointmentCancellation.Subtract(CreatedTime).TotalHours;

                DateTime TimeGate_Refund100_OnCancellation = CreatedTime.AddHours(TimeDiff_CreatedTimeUntilCancellationBlockTime / 2);

                if (CancelTime < TimeGate_Refund100_OnCancellation)
                {
                    return new TablesAppointmentRefundResponse()
                    {
                        TablesAppointmentModel = model,
                        RefundAmount = (decimal) tablesAppointment.Price,
                        RefundStatus = RefundStatus.refund_100_percentage_of_total,
                        Message = "Refund 100%",
                        NumerOfTablesCancelledThisWeek = cancelledTablesAppointmentsWithinThisWeek,
                        CancellationTime = CancelTime,
                        Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                        Cancellation_PartialRefund_TimeGate = TimeGate_Refund100_OnCancellation,
                        CancelUserId = bookingPayments.FirstOrDefault(p => p.UserId == userId)?.UserId,
                        InvitedUserId = bookingPayments.FirstOrDefault(p => p.UserId != userId)?.UserId,
                    };
                }
                else
                {
                    return new TablesAppointmentRefundResponse()
                    {
                        TablesAppointmentModel = model,
                        RefundAmount = (decimal) (tablesAppointment.Price / 2),
                        RefundStatus = RefundStatus.refund_50_percentage_of_total,
                        Message = "Refund 50%",
                        NumerOfTablesCancelledThisWeek = cancelledTablesAppointmentsWithinThisWeek,
                        CancellationTime = CancelTime,
                        Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                        Cancellation_PartialRefund_TimeGate = TimeGate_Refund100_OnCancellation,
                        CancelUserId = bookingPayments.FirstOrDefault(p => p.UserId == userId)?.UserId,
                        InvitedUserId = bookingPayments.FirstOrDefault(p => p.UserId != userId)?.UserId,
                    };
                }
            }
            else if (CancelTime < TimeGate_Refund50_OnCancellation)
            {
                return new TablesAppointmentRefundResponse()
                {
                    TablesAppointmentModel = model,
                    RefundAmount = (decimal)tablesAppointment.Price,
                    RefundStatus = RefundStatus.refund_100_percentage_of_total,
                    Message = "Hoàn tiền 100%",
                    NumerOfTablesCancelledThisWeek = cancelledTablesAppointmentsWithinThisWeek,
                    CancellationTime = CancelTime,
                    Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                    Cancellation_PartialRefund_TimeGate = TimeGate_Refund50_OnCancellation,
                    CancelUserId = bookingPayments.FirstOrDefault(p => p.UserId == userId)?.UserId,
                    InvitedUserId = bookingPayments.FirstOrDefault(p => p.UserId != userId)?.UserId,
                };
            }

            return new TablesAppointmentRefundResponse()
            {
                TablesAppointmentModel = model,
                RefundAmount = (decimal)(tablesAppointment.Price / 2),
                RefundStatus = RefundStatus.refund_50_percentage_of_total,
                Message = "Refund 50%",
                NumerOfTablesCancelledThisWeek = cancelledTablesAppointmentsWithinThisWeek,
                CancellationTime = CancelTime,
                Cancellation_Block_TimeGate = TimeGate_BlockAppointmentCancellation,
                Cancellation_PartialRefund_TimeGate = TimeGate_Refund50_OnCancellation,
                CancelUserId = bookingPayments.FirstOrDefault(p => p.UserId == userId)?.UserId,
                InvitedUserId = bookingPayments.FirstOrDefault(p => p.UserId != userId)?.UserId,
            };
        }

        public async Task<TablesAppointmentModel> UpdateTablesAppointmentAsync(TablesAppointmentModel appointmentModel, int id)
        {
            try
            {
                var tablesAppointment = _mapper.Map<TablesAppointment>(appointmentModel);
                var result = await _tablesAppointmentRepository.UpdateTablesAppointmentAsync(tablesAppointment, id);

                var mappedResult = _mapper.Map<TablesAppointmentModel>(result);

                return mappedResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsByUserId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentsFromUserByUserId(id, parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);

                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<TablesAppointmentResponse>> GetAllTablesAppointmentsJoinedByUserId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllTablesAppointmentsInvitedToUserByUserId(id, parameters);
                var mapped = _mapper.Map<PagedList<TablesAppointmentResponse>>(result);

                return new PagedList<TablesAppointmentResponse>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> UpdateStatusForExpiredAndIncomingTablesAppointments()
        {
            try
            {
                return await _tablesAppointmentRepository.UpdateStatusForExpiredAndIncomingTablesAppointments();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TablesAppointmentModel>> GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests()
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetConfirmedTablesAppointmentsWithRejectedOrExpiredAppointmentRequests();
            
                return _mapper.Map<List<TablesAppointmentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<string> CreateCheckinQrCode(int userId, int tablesAppointmentId)
        {
            try
            {
                string payloadUrl = $"https://backend-production-ac5e.up.railway.app/api/tables-appointment/check-in/{tablesAppointmentId}/users/{userId}";

                using var qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payloadUrl, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                string base64Qr = Convert.ToBase64String(qrCodeBytes);
                return $"data:image/png;base64,{base64Qr}";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<decimal> GetTotalPriceOfPaidTablesAppointmentWithinAMonthOfYearAsync(int month, int year)
        {
            try
            {
                var result = await _tablesAppointmentRepository.GetAllPaidTablesAppointmentWithinAMonthInYearAsync(month, year);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TablesAppointmentsMonthResponse> GetAllBookedTablesAppointmentWithinAMonthInYearAsync(int month, int year)
        {
            try
            {
                var allTAs = await _tablesAppointmentRepository.GetAllBookedTablesAppointmentWithinAMonthInYearAsync(month, year);
                List<TablesAppointmentsDailyResponse> taDailyResponses = new();

                int dayInMonth = DateTime.DaysInMonth(year, month);
                for (int i = 1; i <= dayInMonth; ++i)
                {
                    int booked = allTAs.Where(ta => ta.CreatedAt.Value.Day == i).Count();
                    int cancelled = allTAs.Where(ta => ta.Status == AppointmentStatus.cancelled && ta.CreatedAt.Value.Day == i).Count();
                    int completed = allTAs.Where(ta => ta.Status == AppointmentStatus.completed && ta.CreatedAt.Value.Day == i).Count();
                    int expired = allTAs.Where(ta => ta.Status == AppointmentStatus.expired && ta.CreatedAt.Value.Day == i).Count();
                    int others = allTAs.Where(ta =>
                                        ta.Status != AppointmentStatus.cancelled 
                                        && ta.Status != AppointmentStatus.completed 
                                        && ta.Status != AppointmentStatus.expired
                                        && ta.CreatedAt.Value.Day == i).Count();

                    taDailyResponses.Add(new() 
                    { 
                        DayOfMonth = i, 
                        TablesAppointmentBooked = booked, 
                        CancelledCount = cancelled,
                        CompletedCount = completed,
                        ExpiredCount = expired,
                        FutureCount = others
                    });
                }

                return new()
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalDays = dayInMonth,
                    TablesAppointmentsDailyResponse = taDailyResponses
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
