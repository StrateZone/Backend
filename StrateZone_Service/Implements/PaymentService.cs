using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Repository.Parameters;
﻿using AutoMapper;
using StrateZone_Service.BusinessModels;
using System.Globalization;
using StrateZone_Repository.Pagination;
using static StrateZone_Repository.Parameters.PostgreEnums;
using StrateZone_Repository.Implements;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace StrateZone_Service.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IAppointmentrequestRepository _appointmentrequestRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IPriceService _priceService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITableRepository _tableRepository;
        private readonly ISystemService _systemService;

        public PaymentService(
            IAppointmentRepository appointmentRepository,
            ITablesAppointmentRepository tablesAppointmentRepository,
            IAppointmentrequestRepository appointmentrequestRepository,
            IPaymentRepository paymentRepository,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper,
            IUserRepository userRepository,
            INotificationService notificationService,
            IPriceService priceService,
            IServiceScopeFactory serviceScopeFactory,
            ITableRepository tableRepository,
            ISystemService systemService)
        {
            _appointmentRepository = appointmentRepository;
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _appointmentrequestRepository = appointmentrequestRepository;
            _paymentRepository = paymentRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _priceService = priceService;
            _serviceScopeFactory = serviceScopeFactory;
            _tableRepository = tableRepository;
            _systemService = systemService;
        }

        public async Task<ApiResponse<AppointmentModel>> CreatePaymentBooking(AppointmentModel appointment)
        {
            try
            {
                var userWallet = await _walletRepository.GetWalletByUserIdAsync(appointment.UserId);
                if (userWallet.Balance < appointment.TotalPrice)
                {
                    return new ApiResponse<AppointmentModel>
                    {
                        Success = true,
                        StatusCode = 500,
                        Message = "Payment failed due to not enough in balance",
                        Data = null
                    };
                }

                foreach (var tablesAppointment in appointment.TablesAppointments)
                {
                    PaymentModel paymentModel = new()
                    {
                        UserId = appointment.UserId,
                        TablesAppointmentId = tablesAppointment.Id,
                        PaymentStatus = PostgreEnums.PaymentStatus.paid.ToString(),
                        Description = $"Thanh toán cho bàn {tablesAppointment.Id}",
                        PaymentType = PostgreEnums.PaymentType.appointment.ToString()
                    };

                    await CreatePaymentAsync(paymentModel);
                }

                userWallet.Balance -= appointment.TotalPrice;
                await _walletRepository.UpdateWalletAsync(userWallet, userWallet.WalletId);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                    var newTransaction = new StrateZone_Repository.Entities.Transaction
                    {
                        OfUser = appointment.UserId,
                        Amount = appointment.TotalPrice,
                        Content = "Đã thanh toán đơn " + appointment.AppointmentId + ": " + appointment.TotalPrice,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        TransactionType = PostgreEnums.TransactionType.payment
                    };

                    await service.SaveTransaction(newTransaction);
                });

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    NotificationRequest thisUser = new()
                    {
                        ToUser = appointment.UserId,
                        Title = $"Mã đơn #{appointment.AppointmentId} đã được thanh toán!",
                        Content = $"Đơn đặt bàn với mã đơn #{appointment.AppointmentId} của bạn đều đã được thanh toán! Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi."
                    };

                    await service.CreateNotificationAsync(thisUser);
                });

                var user = await _userRepository.GetUserByIdAsync(appointment.UserId);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var email = new EmailRequest
                    {
                        Subject = "Chi tiết đặt hẹn của bạn tại StrateZone.",
                        ToEmail = user.Email,
                        Content = $@"
                                <html>
                                <head>
                                    <style>
                                        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                                        .container {{ padding: 20px; border: 1px solid #ddd; border-radius: 10px; width: 600px; margin: auto; background-color: #f9f9f9; }}
                                        h2 {{ color: #2c3e50; }}
                                        p {{ font-size: 14px; color: #555; }}
                                        .details, .tables, .requests {{ background-color: #fff; padding: 10px; border-radius: 5px; margin-bottom: 10px; border: 1px solid #ddd; }}
                                        .footer {{ margin-top: 20px; font-size: 12px; color: #777; text-align: center; }}

                                        /* Table Styling */
                                        table {{ width: 100%; border-collapse: collapse; margin-top: 10px; }}
                                        th, td {{ border: 2px solid #333; padding: 10px; text-align: left; }} /* Đường viền rõ hơn */
                                        th {{ background-color: #2c3e50; color: white; }}
                                        tr:nth-child(even) {{ background-color: #f2f2f2; }}
                                        tr:hover {{ background-color: #ddd; }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <h2>Chi tiết lịch hẹn</h2>
                                        <p>Thân gửi {user.Username},</p>
                                        <p>Lịch hẹn của bạn đã được đặt thành công. Dưới đây là thông tin chi tiết:</p>

                                        <div class='details'>
                                            <p><strong>Được tạo vào lúc:</strong> {appointment.CreatedAt?.ToString("HH:mm dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("vi-VN"))}</p>
                                            <p><strong>Tổng cộng:</strong> {appointment.TotalPrice} VNĐ</p>
                                        </div>

                                        <div class='tables'>
                                            <h3>Các bàn đã đặt: </h3>
                                            <table>
                                                <tr>
                                                    <th>Bàn số</th>
                                                    <th>Thời gian bắt đầu</th>
                                                    <th>Thời gian kết thúc</th>
                                                    <th>Giá</th>
                                                </tr>
                                                {string.Join("", appointment.TablesAppointments.Select(t => $@"
                                                    <tr>
                                                        <td>{t.TableId}</td>
                                                        <td>{t.ScheduleTime.ToString("HH:mm dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("vi-VN"))}</td>
                                                        <td>{t.EndTime.ToString("HH:mm dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("vi-VN"))}</td>
                                                        <td>{t.Price:N0} VNĐ</td>
                                                    </tr>
                                                "))}
                                            </table>
                                        </div>

                                        {(!appointment.Appointmentrequests.Any() ? "" : $@"
                                        <div class='requests'>
                                            <h3>Các lời mời đã gửi</h3>
                                            <table>
                                                <tr>
                                                    <th>Đến</th>
                                                    <th>Bàn số</th>
                                                    <th>Hết hạn vào lúc</th>
                                                </tr>
                                                {string.Join("", appointment.Appointmentrequests.Select(r => $@"
                                                    <tr>
                                                        <td>{r.ToUserNavigation.Username}</td>
                                                        <td>{r.TableId}</td>
                                                        <td>{r.ExpireAt?.ToString("HH:mm dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("vi-VN"))}</td>
                                                    </tr>
                                                "))}
                                            </table>
                                        </div>
                                        ")}

                                        <p>Nếu bạn có thắc mắc, vui lòng liên hệ chúng tôi.</p>
                                        <p>Trân trọng,<br/>StrateZone</p>

                                        <div class='footer'>
                                            <p>Đây là tin nhắn tự động, xin đừng trả lời.</p>
                                        </div>
                                    </div>
                                </body>
                                </html>
                            "
                    };
                    await service.SendEmailAsync(email);
                });

                var result = new ApiResponse<AppointmentModel>
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Payment success",
                    Data = appointment
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApiResponse<AppointmentrequestModel>> CreateAppointmentRequestPaymentBooking(AppointmentrequestPaymentRequest appointmentrequestModel)
        {
            try
            {
                var tableAppointment = await _tablesAppointmentRepository.GetByIdAsync(appointmentrequestModel.TableAppointmentId);
                if (tableAppointment.Status != AppointmentStatus.confirmed && tableAppointment.Status != AppointmentStatus.pending && tableAppointment.Status != AppointmentStatus.incoming)
                {
                    return new ApiResponse<AppointmentrequestModel>
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = $"This appointment is already {tableAppointment.Status}",
                        Data = null
                    };
                }
                
                var appointment_request = (await _appointmentrequestRepository.GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(appointmentrequestModel.FromUser, tableAppointment.Id))
                                            .SingleOrDefault(ar => ar.ToUser == appointmentrequestModel.ToUser && ar.Status == RequestStatus.pending);

                if (appointment_request == null)
                {
                    return new ApiResponse<AppointmentrequestModel>
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = $"This appointment invitation is no longer available.",
                        Data = null
                    };
                }

                if (appointment_request.Status == RequestStatus.expired || appointment_request.Status == RequestStatus.cancelled || appointment_request.Status == RequestStatus.rejected)
                {
                    return new ApiResponse<AppointmentrequestModel>
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = $"This appointment invitation is already {appointment_request.Status}",
                        Data = null
                    };
                }

                if (tableAppointment.PaidForOpponent)
                {
                    var requestAcceptor = await _userRepository.GetUserByIdAsync(appointmentrequestModel.ToUser);
                    var requestSender = await _userRepository.GetUserByIdAsync(appointmentrequestModel.FromUser);

                    await _appointmentrequestRepository.AcceptAppointmentrequestAsync(appointment_request.Id);

                    _ = Task.Run(async () =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        NotificationRequest notificationToUser = new()
                        {
                            ToUser = appointmentrequestModel.ToUser,
                            Title = $"Bạn đã chấp nhận lời mời từ {requestSender.Username}!",
                            Content = $"Bạn đã chấp nhận lời mời chơi cờ đến từ {requestSender.Username} (đơn #{tableAppointment.AppointmentId}, bàn {tableAppointment.TableId}). " +
                            $"Lịch hẹn của hai bạn sẽ diễn ra vào lúc {tableAppointment.ScheduleTime.TimeOfDay}, ngày {DateOnly.FromDateTime(tableAppointment.ScheduleTime)}. " +
                            $"Chúc hai bạn có một trải nghiệm chơi cờ vui vẻ!",
                            Type = NotificationType.appointment_request_from
                        };

                        NotificationRequest notificationFromUser = new()
                        {
                            ToUser = appointmentrequestModel.FromUser,
                            Title = $"{requestAcceptor.Username} đã chấp nhận lời mời chơi cờ của bạn!",
                            Content = $"{requestAcceptor.Username} đã chấp nhận lời mời của bạn gửi đến họ (đơn #{tableAppointment.AppointmentId}, bàn {tableAppointment.TableId}). " +
                            $"Lịch hẹn của hai bạn sẽ diễn ra vào lúc {tableAppointment.ScheduleTime.TimeOfDay}, ngày {DateOnly.FromDateTime(tableAppointment.ScheduleTime)}. " +
                            $"Chúc hai bạn có một trải nghiệm chơi cờ vui vẻ!",
                            Type = NotificationType.appointment_request_to
                        };

                        await service.CreateNotificationsAsync([notificationFromUser, notificationToUser]);
                    });

                    return new ApiResponse<AppointmentrequestModel>
                    {
                        Success = true,
                        StatusCode = 201,
                        Message = "Request accepted",
                        Data = null
                    };
                }
                else
                {
                    var requestAcceptor = await _userRepository.GetUserByIdAsync(appointmentrequestModel.ToUser);
                    var requestSender = await _userRepository.GetUserByIdAsync(appointmentrequestModel.FromUser);
                    var userWallet = await _walletRepository.GetWalletByUserIdAsync(requestAcceptor.UserId);

                    if (userWallet.Balance < tableAppointment.Price)
                    {
                        return new ApiResponse<AppointmentrequestModel>
                        {
                            Success = false,
                            StatusCode = 500,
                            Message = "Balance is not enough",
                            Data = null
                        };
                    }

                    await _appointmentrequestRepository.AcceptAppointmentrequestAsync(appointment_request.Id);
                    await _walletRepository.WithdrawalWalletAsync((int)tableAppointment.Price, userWallet.WalletId);

                    var invitedUserPayment = new Payment()
                    {
                        UserId = appointmentrequestModel.ToUser,
                        TablesAppointmentId = tableAppointment.Id,
                        PaymentStatus = PostgreEnums.PaymentStatus.paid,
                        Description = $"Thanh toán cho bàn {tableAppointment.Id}"
                                    + (requestSender != null ? $"(chơi chung với {requestSender.Username})" : ""),
                        PaymentType = PostgreEnums.PaymentType.appointment
                    };

                    await _paymentRepository.CreatePaymentAsync(invitedUserPayment);

                    var invitorUserPayment = (await GetPaymentsByTablesAppointmentIdAsync(tableAppointment.Id))
                                            .SingleOrDefault(p => p.UserId == requestSender.UserId);

                    invitorUserPayment.Description = $"Thanh toán cho bàn {tableAppointment.Id}"
                                    + (requestAcceptor != null ? $"(chơi chung với {requestAcceptor.Username})" : "");
                    await UpdatePaymentAsync(invitorUserPayment, invitorUserPayment.Id);

                    _ = Task.Run(async () =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                        var newTransaction = new StrateZone_Repository.Entities.Transaction
                        {
                            OfUser = appointmentrequestModel.ToUser,
                            Amount = tableAppointment.Price,
                            Content = "Thanh toán cho đơn mời " + tableAppointment.AppointmentId + ": " + tableAppointment.Price + " VND",
                            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                            TransactionType = PostgreEnums.TransactionType.payment,
                        };
                        await repo.SaveTransaction(newTransaction);
                    });

                    _ = Task.Run(async () =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        NotificationRequest notificationToUser = new()
                        {
                            ToUser = appointmentrequestModel.ToUser,
                            Title = $"Bạn đã hoàn thành thanh toán đơn mời của {requestSender.Username}!",
                            Content = $"Bạn đã hoàn tất thanh toán cho đơn mời đến từ {requestSender.Username} (đơn #{tableAppointment.AppointmentId}, bàn {tableAppointment.TableId}). " +
                            $"Lịch hẹn của hai bạn sẽ diễn ra vào lúc {tableAppointment.ScheduleTime.TimeOfDay}, ngày {DateOnly.FromDateTime(tableAppointment.ScheduleTime)}. " +
                            $"Chúc hai bạn có một trải nghiệm chơi cờ vui vẻ!",
                            Type = NotificationType.appointment_request_from
                        };

                        NotificationRequest notificationFromUser = new()
                        {
                            ToUser = appointmentrequestModel.FromUser,
                            Title = $"{requestAcceptor.Username} đã chấp nhận lời mời chơi cờ của bạn!",
                            Content = $"{requestAcceptor.Username} đã chấp nhận & hoàn tất thanh toán cho đơn mời của bạn gửi đến họ (đơn #{tableAppointment.AppointmentId}, bàn {tableAppointment.TableId}). " +
                            $"Lịch hẹn của hai bạn sẽ diễn ra vào lúc {tableAppointment.ScheduleTime.TimeOfDay}, ngày {DateOnly.FromDateTime(tableAppointment.ScheduleTime)}. " +
                            $"Chúc hai bạn có một trải nghiệm chơi cờ vui vẻ!",
                            Type = NotificationType.appointment_request_to
                        };

                        await service.CreateNotificationsAsync([notificationFromUser, notificationToUser]);
                    });

                    return new ApiResponse<AppointmentrequestModel>
                    {
                        Success = true,
                        StatusCode = 201,
                        Message = "Payment success",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // method only used when the appointment owner chooses to pay the rest of the price
        public async Task<ApiResponse<TablesAppointmentModel>> CreateExtendedTablesAppointmentPaymentBooking(TablesAppointmentPaymentRequest request)
        {
            try
            {
                DateTime Now = DateTime.UtcNow.AddHours(7);

                var system = await _systemService.GetSystemsByIdAsync(1);

                double durationInMinutes = request.EndTime.Subtract(request.StartTime).TotalMinutes;
                int Min_ExtendTime = system.Min_Minutes_For_TablesExtend;
                int Max_ExtendTime = system.Max_Minutes_For_TablesExtend;
                if (durationInMinutes < Min_ExtendTime)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = $"Thời gian mở rộng tối thiểu là {Min_ExtendTime} phút.",
                        Data = null
                    };
                }
                else if (durationInMinutes > Max_ExtendTime)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = $"Thời gian mở rộng tối đa là {Max_ExtendTime} phút.",
                        Data = null
                    };
                }

                var avTables = (await _tableRepository.GetAvailableTablesAsync(request.StartTime, request.EndTime)).Select(t => t.TableId);
                if (!avTables.Contains(request.TableId))
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Bàn không khả dụng.",
                        Data = null
                    };
                }

                var oldTableAppointment = await _tablesAppointmentRepository.GetByIdAsync(request.OldTablesAppointmentId);
                if (oldTableAppointment.Status != AppointmentStatus.checked_in)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Bàn chưa được check-in không được phép gia hạn thêm giờ chơi.",
                        Data = null
                    };
                }
                else if (Now.AddMinutes(system.ExtendAllow_BeforeMinutes_FromTableComplete) < oldTableAppointment.EndTime)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = $"Gia hạn thêm giờ chơi chỉ mở {system.ExtendAllow_BeforeMinutes_FromTableComplete} phút trước giờ kết thúc của giờ hiện tại.",
                        Data = null
                    };
                }
                else if (oldTableAppointment.IsExtended && oldTableAppointment.ExtendedCount >= system.Max_Tables_Extends_Count)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = $"Mỗi bàn chỉ có thể gia hạn giờ chơi tối đa {system.Max_Tables_Extends_Count} lần.",
                        Data = null
                    };
                }

                var appointment = await _appointmentRepository.GetAppointmentByIdAsync((int)oldTableAppointment.AppointmentId);
                if (appointment.UserId != request.UserId)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = $"Chỉ có chủ đơn mới được quyền gia hạn thêm thời gian.",
                        Data = null
                    };
                }

                var userWallet = await _walletRepository.GetWalletByUserIdAsync(request.UserId);

                if (userWallet.Balance < request.Price)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Balance is not enough",
                        Data = null
                    };
                }

                await _walletRepository.WithdrawalWalletAsync((int)request.Price, userWallet.WalletId);

                oldTableAppointment.ExtendedCount++;
                oldTableAppointment.IsExtended = true;

                var tablesAppointment = await _tablesAppointmentRepository.CreateTablesAppointmentAsync(
                        new()
                        { 
                            AppointmentId = request.AppointmentId,
                            TableId = request.TableId,
                            Price = request.Price,
                            ScheduleTime = request.StartTime,
                            EndTime = request.EndTime,
                            PaidForOpponent = false,
                            Status = AppointmentStatus.confirmed,
                            IsExtended = true,
                            ExtendedOf = oldTableAppointment.Id,
                            ExtendedCount = oldTableAppointment.ExtendedCount,
                            Note = $"Đơn mở rộng cho bàn có mã đặt {oldTableAppointment.Id} (đơn số #{oldTableAppointment.AppointmentId}, bàn {oldTableAppointment.TableId})",
                        }
                    );

                oldTableAppointment.Note = $"Khách đã yêu cầu thêm giờ chơi. Mã đặt bàn mới: {tablesAppointment.Id} (đơn số #{tablesAppointment.AppointmentId}, bàn {tablesAppointment.TableId})";
                await _tablesAppointmentRepository.UpdateTablesAppointmentAsync(oldTableAppointment, oldTableAppointment.Id);

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

                    await service.UpdateAppointmentPriceAsync((int)tablesAppointment.AppointmentId);
                });

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                    PaymentModel paymentModel = new()
                    {
                        UserId = request.UserId,
                        TablesAppointmentId = tablesAppointment.Id,
                        PaymentStatus = PostgreEnums.PaymentStatus.paid.ToString(),
                        Description = $"Thanh toán cho bàn {tablesAppointment.Id}",
                        PaymentType = PostgreEnums.PaymentType.appointment.ToString()
                    };

                    await service.CreatePaymentAsync(paymentModel);
                });

                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                    var newTransaction = new StrateZone_Repository.Entities.Transaction
                    {
                        OfUser = request.UserId,
                        Amount = tablesAppointment.Price,
                        Content = "Đã thanh toán đơn mở rộng " + tablesAppointment.Id + ": " + tablesAppointment.Price,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        TransactionType = PostgreEnums.TransactionType.payment
                    };

                    await service.SaveTransaction(newTransaction);
                });

                return new ApiResponse<TablesAppointmentModel>
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Payment success",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApiResponse<UserResponse>> CreateMembershipPaymentAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return new ApiResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = $"This user does not exist.",
                        Data = null
                    };
                }

                if (user.UserRole != UserRole.RegisteredUser)
                {
                    return new ApiResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = $"This user is already a(n) {user.UserRole.ToString()}.",
                        Data = null
                    };
                }

                var membershipPrice = await _priceService.GetMembershipPriceAsync();
                var userWallet = await _walletRepository.GetWalletByUserIdAsync(user.UserId);

                if (userWallet.Balance < membershipPrice.Price1)
                {
                    return new ApiResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = "Balance is not enough",
                        Data = null
                    };
                }

                await _walletRepository.WithdrawalWalletAsync((int)membershipPrice.Price1, userWallet.WalletId);
                
                user.UserRole = UserRole.Member;
                user.MembershipExpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7).AddDays(30), DateTimeKind.Unspecified);
                await _userRepository.UpdateUserAsync(user, user.UserId);

                var userMembershipPayment = new Payment()
                {
                    UserId = user.UserId,
                    PaymentStatus = PostgreEnums.PaymentStatus.paid,
                    Description = "Đăng kí gói thành viên",
                    PaymentType = PostgreEnums.PaymentType.membership
                };

                await _paymentRepository.CreatePaymentAsync(userMembershipPayment);

                var newTransaction = new StrateZone_Repository.Entities.Transaction
                {
                    OfUser = user.UserId,
                    Amount = membershipPrice.Price1,
                    Content = "Đăng kí gói thành viên",
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    TransactionType = PostgreEnums.TransactionType.payment,
                };
                await _transactionRepository.SaveTransaction(newTransaction);

                NotificationRequest notificationToUser = new()
                {
                    ToUser = user.UserId,
                    Title = $"Đăng kí gói thành viên thành công!",
                    Content = $"Bạn đã trở thành thành viên của CLB StrateZone, bây giờ bạn đã có thể tham gia tương tác " +
                    $"với các thành viên của câu lạc bộ!",
                    Type = NotificationType.community
                };
                await _notificationService.CreateNotificationAsync(notificationToUser);

                return new ApiResponse<UserResponse>
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Payment success",
                    Data = _mapper.Map<UserResponse>(user)
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaymentModel> CreatePaymentAsync(PaymentModel paymentModel)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentModel);
                var result = await _paymentRepository.CreatePaymentAsync(payment);
                return _mapper.Map<PaymentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PaymentModel>> GetPaymentsByTablesAppointmentIdAsync(int id)
        {
            try
            {
                var result = await _paymentRepository.GetPaymentsByTablesAppointmentIdAsync(id);
                return _mapper.Map<List<PaymentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<PaymentModel>> GetPaymentsByUserIdAsync(int id, PaymentParameters parameters)
        {
            try
            {
                var result = await _paymentRepository.GetPaymentsByUserIdAsync(id, parameters);
                var mapped = _mapper.Map<PagedList<PaymentModel>>(result);
            
                return new PagedList<PaymentModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaymentModel> UpdatePaymentAsync(PaymentModel paymentModel, int id)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentModel);
                var result = await _paymentRepository.UpdatePaymentAsync(payment, id);
                return _mapper.Map<PaymentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<PaymentModel>> GetPaymentsAsync(PaymentParameters parameters)
        {
            try
            {
                var result = await _paymentRepository.GetPaymentsAsync(parameters);
                var mapped = _mapper.Map<PagedList<PaymentModel>>(result);

                return new PagedList<PaymentModel>(mapped, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetMembershipPaymentsWithinAMonthInYearAsync(int month, int year)
        {
            try
            {
                return await _paymentRepository.GetMembershipPaymentsWithinAMonthInYearAsync(month, year);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetMembershipPaymentsWithinADayInYearAsync(int day, int month, int year)
        {
            try
            {
                var result = await _paymentRepository.GetMembershipPaymentsWithinADayInYearAsync(day, month, year);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetReportMembershipPaymentsWithinADayInYearAsync(int month, int year)
        {
            try
            {
                return await _paymentRepository.GetMembershipPaymentsWithinAMonthInYearAsync(month, year);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
