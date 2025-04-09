using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Repository.Parameters;
﻿using AutoMapper;
using StrateZone_Service.BusinessModels;
using System.Globalization;
using MealHunt_Repositories.Pagination;
using static StrateZone_Repository.Parameters.PostgreEnums;
using StrateZone_Repository.Implements;

namespace StrateZone_Service.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IAppointmentrequestRepository _appointmentrequestRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(
            ITablesAppointmentRepository tablesAppointmentRepository,
            IAppointmentrequestRepository appointmentrequestRepository,
            IPaymentRepository paymentRepository,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserRepository userRepository,
            IAppointmentRepository appointmentRepository,
            INotificationService notificationService)
        {
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _appointmentrequestRepository = appointmentrequestRepository;
            _paymentRepository = paymentRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
            _notificationService = notificationService;
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

                userWallet.Balance -= appointment.TotalPrice;
                await _walletRepository.UpdateWalletAsync(userWallet, userWallet.WalletId);
                foreach (var tablesAppointment in appointment.TablesAppointments)
                {
                    tablesAppointment.Status = AppointmentStatus.confirmed.ToString();
                    var mappedTA = _mapper.Map<TablesAppointment>(tablesAppointment);
                    await _tablesAppointmentRepository.UpdateTablesAppointmentAsync(mappedTA, tablesAppointment.Id);

                    var updatingPayment = (await _paymentRepository.GetPaymentsByTablesAppointmentIdAsync(tablesAppointment.Id)).SingleOrDefault(p => p.UserId == appointment.UserId);
                    var mappedPayment = _mapper.Map<PaymentModel>(updatingPayment);
                    mappedPayment.PaymentStatus = PostgreEnums.PaymentStatus.paid.ToString();
                    await UpdatePaymentAsync(mappedPayment, mappedPayment.Id);
                }

                var newTransaction = new Transaction
                {
                    OfUser = appointment.UserId,
                    Amount = appointment.TotalPrice,
                    Content = "Đã thanh toán đơn " + appointment.AppointmentId + ": " + appointment.TotalPrice,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    TransactionType = PostgreEnums.TransactionType.payment
                };

                await _transactionRepository.SaveTransaction(newTransaction);

                appointment.Status = AppointmentStatus.incompleted.ToString();
                var mapped = _mapper.Map<Appointment>(appointment);
                await _appointmentRepository.UpdateAppointmentAsync(mapped, appointment.AppointmentId);

                NotificationRequest thisUser = new()
                {
                    ToUser = appointment.UserId,
                    Title = $"Mã đơn #{appointment.AppointmentId} đã được thanh toán!",
                    Content = $"Đơn đặt bàn với mã đơn #{appointment.AppointmentId} của bạn đều đã được thanh toán! Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi."
                };
                await _notificationService.CreateNotificationAsync(thisUser);

                foreach (var appointmentRequest in appointment.Appointmentrequests)
                {
                    if(appointmentRequest.Status == RequestStatus.accepted.ToString())
                    {
                        NotificationRequest toUser = new()
                        {
                            ToUser = appointmentRequest.ToUser,
                            Title = $"Mã đơn #{appointment.AppointmentId} đã được người mời thanh toán!",
                            Content = $"Đơn đặt bàn với mã đơn #{appointment.AppointmentId} của bạn đã được người mời thanh toán thanh toán. Hãy tiến hành thanh toán phần của bạn! Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi."
                        };
                        await _notificationService.CreateNotificationAsync(thisUser);
                    }
                }

                var user = await _userRepository.GetUserByIdAsync(appointment.UserId);

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

                await _emailService.SendEmailAsync(email);

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
                var tableAppointment = await _tablesAppointmentRepository.GetTablesAppointmentByTableIdAndAppointmentIdAsync(appointmentrequestModel.TableId, (int)appointmentrequestModel.AppointmentId);
                var appointment_request = (await _appointmentrequestRepository.GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(appointmentrequestModel.FromUser, tableAppointment.Id))
                                            .SingleOrDefault(ar => ar.ToUser == appointmentrequestModel.ToUser);

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

                var invitedUser = await _userRepository.GetUserByIdAsync(appointmentrequestModel.ToUser);
                var invitorUser = await _userRepository.GetUserByIdAsync(appointmentrequestModel.FromUser);

                var userWallet = await _walletRepository.GetWalletByUserIdAsync(appointmentrequestModel.ToUser);

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

                await _walletRepository.WithdrawalWalletAsync((int)tableAppointment.Price, userWallet.WalletId);

                var payment = (await _paymentRepository.GetPaymentsByTablesAppointmentIdAsync(tableAppointment.Id)).SingleOrDefault(p => p.UserId == appointmentrequestModel.ToUser);
                payment.PaymentStatus = PostgreEnums.PaymentStatus.paid;
                await _paymentRepository.UpdatePaymentAsync(payment, payment.Id);

                var newTransaction = new Transaction
                {
                    OfUser = appointmentrequestModel.ToUser,
                    Amount = tableAppointment.Price,
                    Content = "Thanh toán cho đơn mời " + tableAppointment.AppointmentId + ": " + tableAppointment.Price + " VND",
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    TransactionType = PostgreEnums.TransactionType.payment,
                };
                await _transactionRepository.SaveTransaction(newTransaction);

                NotificationRequest notificationToUser = new()
                {
                    ToUser = appointmentrequestModel.ToUser,
                    Title = $"Bạn đã hoàn thành thanh toán đơn mời của {invitorUser.Username}!",
                    Content = $"Bạn đã hoàn tất thanh toán cho đơn mời đến từ {invitorUser.Username} (đơn #{tableAppointment.AppointmentId}, bàn {tableAppointment.TableId}). " +
                    $"Lịch hẹn của hai bạn sẽ diễn ra vào lúc {tableAppointment.ScheduleTime.TimeOfDay}, ngày {DateOnly.FromDateTime(tableAppointment.ScheduleTime)}. " +
                    $"Chúc hai bạn có một trải nghiệm chơi cờ vui vẻ!",
                    Type = NotificationType.appointment_request_from
                };
                await _notificationService.CreateNotificationAsync(notificationToUser);

                NotificationRequest notificationFromUser = new()
                {
                    ToUser = appointmentrequestModel.FromUser,
                    Title = $"{invitedUser.Username} đã hoàn thành thanh toán đơn mời!",
                    Content = $"{invitedUser.Username} đã hoàn tất thanh toán cho đơn mời của bạn gửi đến họ (đơn #{tableAppointment.AppointmentId}, bàn {tableAppointment.TableId}). " +
                    $"Lịch hẹn của hai bạn sẽ diễn ra vào lúc {tableAppointment.ScheduleTime.TimeOfDay}, ngày {DateOnly.FromDateTime(tableAppointment.ScheduleTime)}. " +
                    $"Chúc hai bạn có một trải nghiệm chơi cờ vui vẻ!",
                    Type = NotificationType.appointment_request_to
                };
                await _notificationService.CreateNotificationAsync(notificationFromUser);

                return new ApiResponse<AppointmentrequestModel>
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

        // method only used when the appointment owner chooses to pay the rest of the price
        public async Task<ApiResponse<TablesAppointmentModel>> CreateTablesAppointmentPaymentBooking(TablesAppointmentPaymentRequest appointmentrequestModel)
        {
            try
            {
                var tableAppointment = await _tablesAppointmentRepository.GetTablesAppointmentByTableIdAndAppointmentIdAsync(appointmentrequestModel.TableId, (int)appointmentrequestModel.AppointmentId);

                var userWallet = await _walletRepository.GetWalletByUserIdAsync(appointmentrequestModel.UserId);

                if (userWallet.Balance < tableAppointment.Price)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = "Balance is not enough",
                        Data = null
                    };
                }

                await _walletRepository.WithdrawalWalletAsync((int)tableAppointment.Price, userWallet.WalletId);

                var payment = (await _paymentRepository.GetPaymentsByTablesAppointmentIdAsync(tableAppointment.Id)).SingleOrDefault(p => p.UserId == appointmentrequestModel.UserId && p.PaymentStatus == PaymentStatus.unpaid);
                
                if (payment == null)
                {
                    return new ApiResponse<TablesAppointmentModel>
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = "No payment was found",
                        Data = null
                    };
                }

                payment.PaymentStatus = PostgreEnums.PaymentStatus.paid;
                await _paymentRepository.UpdatePaymentAsync(payment, payment.Id);

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
    }
}
