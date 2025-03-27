using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Repository.Parameters;
﻿using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrateZone_Service.BusinessModels;
using System.Globalization;

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
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public PaymentService(IAppointmentRepository appointmentRepository,
            ITablesAppointmentRepository tablesAppointmentRepository,
            IAppointmentrequestRepository appointmentrequestRepository,
            IPaymentRepository paymentRepository,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _appointmentrequestRepository = appointmentrequestRepository;
            _paymentRepository = paymentRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
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
                    var updatingPayment = (await _paymentRepository.GetPaymentsByTablesAppointmentIdAsync(tablesAppointment.Id)).SingleOrDefault(p => p.UserId == appointment.UserId);
                    updatingPayment.PaymentStatus = PostgreEnums.PaymentStatus.paid;
                    await _paymentRepository.UpdatePaymentAsync(updatingPayment, updatingPayment.Id);
                }

                var newTransaction = new Transaction
                {
                    OfUser = appointment.UserId,
                    Amount = appointment.TotalPrice,
                    Content = "Paid booking " + appointment.AppointmentId + ": " + appointment.TotalPrice,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    TransactionType = PostgreEnums.TransactionType.payment
                };

                await _transactionRepository.SaveTransaction(newTransaction);

                var user = await _userRepository.GetUserByIdAsync(appointment.UserId);

                var email = new EmailRequest
                {
                    Subject = "Your Appointment Details",
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

        public async Task<List<PaymentModel>> GetPaymentsByUserIdAsync(int id)
        {
            try
            {
                var result = await _paymentRepository.GetPaymentsByUserIdAsync(id);
                return _mapper.Map<List<PaymentModel>>(result);
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
    }
}
