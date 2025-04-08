using AutoMapper;
using Azure.Core;
using MealHunt_Repositories.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAppointmentrequestService _appointmentrequestService;
        private readonly IUserService _userService;
        private readonly ITableService _tableService;
        private readonly ITablesAppointmentService _tablesAppointmentService;
        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly INotificationService _notificationService;
        private readonly IVoucherService _voucherService;
        private readonly IMapper _mapper;
        private readonly ScheduleTimeValidator _scheduleTimeValidator;

        public AppointmentService(IAppointmentRepository appointmentRepository, 
            IUserService userService, 
            ITableService tableService, 
            ITablesAppointmentService tablesAppointmentService, 
            IMapper mapper, 
            IAppointmentrequestService appointmentrequestService, 
            IPaymentService paymentService, 
            IWalletService walletService, 
            ITransactionRepository transactionRepository, 
            ScheduleTimeValidator scheduleTimeValidator, 
            INotificationService notificationService)
        {
            _appointmentRepository = appointmentRepository;
            _userService = userService;
            _mapper = mapper;
            _tableService = tableService;
            _tablesAppointmentService = tablesAppointmentService;
            _appointmentrequestService = appointmentrequestService;
            _paymentService = paymentService;
            _walletService = walletService;
            _transactionRepository = transactionRepository;
            _scheduleTimeValidator = scheduleTimeValidator;
            _notificationService = notificationService;
        }

        public async Task<PagedList<AppointmentResponse>> GetAppointmentsAsync(AppointmentParameters parameters)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentsAsync(parameters);

                var mappedAppointments = _mapper.Map<PagedList<AppointmentResponse>>(result);

                return new PagedList<AppointmentResponse>(mappedAppointments, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentResponse>> GetAllAppointmentsAsync(AppointmentAdminParameters parameters)
        {
            try
            {
                var result = await _appointmentRepository.GetAllAppointmentsAsync(parameters);

                if (parameters.Status != null)
                {
                    foreach (var a in result)
                    {
                        a.TablesAppointments = a.TablesAppointments.Where(ta => ta.Status == parameters.Status).ToList();
                    }
                }

                var appointments = _mapper.Map<PagedList<AppointmentResponse>>(result);

                return new PagedList<AppointmentResponse>(appointments, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentResponse>> GetAllAppointmentsCheckinAsync(AppointmentAdminParameters parameters)
        {
            try
            {
                var result = await _appointmentRepository.GetAllAppointmentsCheckinAsync(parameters);
                var appointments = _mapper.Map<PagedList<AppointmentResponse>>(result);

                return new PagedList<AppointmentResponse>(appointments, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentResponse>> GetAppointmentsByUserIdAsync(AppointmentParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentsByUserIdAsync(parameters, userId);

                var appointments = _mapper.Map<PagedList<AppointmentResponse>>(result);

                return new PagedList<AppointmentResponse>(appointments, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentResponse> GetAppointmentByIdAsync(int id)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentByIdAsync(id);
                return _mapper.Map<AppointmentResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TablesAppointmentRequest>> CheckAppointmentAvailability(AppointmentRequest request)
        {
            foreach (var ta in request.TablesAppointmentRequests)
            {
                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(ta.ScheduleTime, ta.EndTime, false);
                if (!isValid) throw new Exception(errorMessage);
            }

            HashSet<int> requestedTableIds = request.TablesAppointmentRequests.Select(ta => ta.TableId).ToHashSet();
            List<TablesAppointmentRequest> unavailableTables = new();

            foreach (var tablesAppointment in request.TablesAppointmentRequests)
            {
                List<int> availableTableIds = (await _tableService.GetAllAvailableTablesAsync(
                                                            tablesAppointment.ScheduleTime, tablesAppointment.EndTime))
                                              .Select(t => t.TableId)
                                              .ToList();

                var unavailableTableIds = requestedTableIds.Except(availableTableIds);
                foreach (var unavailableTableId in unavailableTableIds)
                {
                    if (tablesAppointment.TableId != unavailableTableId) continue;

                    unavailableTables.Add(
                        new()
                        {
                            Price = 0,
                            TableId = unavailableTableId,
                            ScheduleTime = tablesAppointment.ScheduleTime,
                            EndTime = tablesAppointment.EndTime,
                        }
                    );
                }
            }

            return unavailableTables
                    .GroupBy(t => new { t.TableId, t.ScheduleTime, t.EndTime })
                    .Select(g => g.First())
                    .ToList();
        }

        public async Task<AppointmentModel> CreateAppointmentAsync(CustomModels.RequestModels.AppointmentRequest request)
        {
            try
            {
                AppointmentModel appointmentModel = new AppointmentModel()
                {
                    UserId = request.UserId,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Local),
                    TotalPrice = request.TotalPrice,
                };

                var mappedAppointment = _mapper.Map<Appointment>(appointmentModel);
                var appointment = await _appointmentRepository.CreateAppointmentAsync(mappedAppointment);
                var result = _mapper.Map<AppointmentModel>(appointment);

                foreach (var tablesAppointmentRequest in request.TablesAppointmentRequests)
                {
                    TablesAppointmentModel tablesAppointmentModel = new()
                    {
                        AppointmentId = appointment.AppointmentId,
                        TableId = tablesAppointmentRequest.TableId,
                        ScheduleTime = DateTime.SpecifyKind(tablesAppointmentRequest.ScheduleTime, DateTimeKind.Unspecified),
                        EndTime = DateTime.SpecifyKind(tablesAppointmentRequest.EndTime, DateTimeKind.Unspecified),
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                        Price = tablesAppointmentRequest.Price
                    };

                    result.TablesAppointments.Add(tablesAppointmentModel);
                }

                var tablesAppointments = await _tablesAppointmentService.CreateTablesAppointmentsFromAppointmentAsync(result);
                result.TablesAppointments = tablesAppointments;

                var requests = await _appointmentrequestService.LinkAppointmentrequestsToAppointmentAsync(result);
                result.Appointmentrequests = requests;

                foreach (var tablesAppointment in tablesAppointments)
                {
                    var acceptedUser = await _userService.FindUserAcceptedToJoinTablesAppointment(tablesAppointment);

                    PaymentModel paymentModel = new()
                    {
                        UserId = appointment.UserId,
                        TablesAppointmentId = tablesAppointment.Id,
                        PaymentStatus = PostgreEnums.PaymentStatus.unpaid.ToString(),
                        Description = $"Payment for tables appointment {tablesAppointment.Id}" 
                                + (acceptedUser != null ? $"(shared with user {acceptedUser.UserId})" : ""),
                        PaymentType = PostgreEnums.PaymentType.appointment.ToString()
                    };

                    await _paymentService.CreatePaymentAsync(paymentModel);

                    if (acceptedUser != null)
                    {
                        PaymentModel paymentModel2 = new()
                        {
                            UserId = acceptedUser.UserId,
                            TablesAppointmentId = tablesAppointment.Id,
                            PaymentStatus = PostgreEnums.PaymentStatus.unpaid.ToString(),
                            Description = $"Payment for tables appointment {tablesAppointment.Id} (shared with user {appointment.UserId})",
                            PaymentType = PostgreEnums.PaymentType.appointment.ToString()
                        };

                        await _paymentService.CreatePaymentAsync(paymentModel2);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentModel> UpdateAppointmentAsync(AppointmentModel appointmentModel, int id)
        {
            try
            {
                var appointment = _mapper.Map<Appointment>(appointmentModel);
                var result = await _appointmentRepository.UpdateAppointmentAsync(appointment, id);

                return _mapper.Map<AppointmentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentModel> DeleteAppointmentAsync(int id)
        {
            try
            {
                var result = await _appointmentRepository.DeleteAppointmentAsync(id);
                return _mapper.Map<AppointmentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentModel> RefundAppointment100Async(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
                var userWallet = await _walletService.GetWalletByUserIdAsync(appointment.UserId);
                var tableAppointments = appointment.TablesAppointments.ToList();

                foreach (var tableAppointment in tableAppointments)
                {
                    tableAppointment.Status = AppointmentStatus.refunded;
                    var model = _mapper.Map<TablesAppointmentModel>(tableAppointment);
                    await _tablesAppointmentService.UpdateTablesAppointmentAsync(model, model.Id);
                }

                userWallet.Balance += appointment.TotalPrice;
                await _walletService.UpdateWalletAsync(userWallet, userWallet.WalletId);

                var newAdminTransaction = new Transaction
                {
                    Amount = appointment.TotalPrice,
                    Content = "Refund for booking " + appointment.AppointmentId + ": " + appointment.TotalPrice,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    OfUser = null,
                    TransactionType = TransactionType.refund,
                };

                await _transactionRepository.SaveTransaction(newAdminTransaction);

                var newTransaction = new Transaction
                {
                    Amount = appointment.TotalPrice,
                    Content = "Refunded for booking " + appointment.AppointmentId + ": " + appointment.TotalPrice,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                    OfUser = appointment.UserId,
                    TransactionType = TransactionType.refund,
                };

                await _transactionRepository.SaveTransaction(newTransaction);


                appointment.Status = AppointmentStatus.refunded;
                var updatedAppointment = await _appointmentRepository.UpdateAppointmentAsync(appointment, appointment.AppointmentId);

                return _mapper.Map<AppointmentModel>(updatedAppointment);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateStatusForAppointmentBasedOnTablesAppointments()
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsWithIncompletedStatusToBeCompletedBasedOnTablesAppointments();
            
                foreach (var appointment in appointments)
                {
                    appointment.Status = AppointmentStatus.completed;
                    await _appointmentRepository.UpdateAppointmentAsync(appointment, appointment.AppointmentId);

                    NotificationRequest request = new()
                    {
                        ToUser = appointment.UserId,
                        Title = $"Mã đơn #{appointment.AppointmentId} đã được hoàn thành!",
                        Content = $"Những đơn đặt bàn với mã đơn #{appointment.AppointmentId} của bạn đều đã hoàn tất! Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi."
                    };

                    await _notificationService.CreateNotificationAsync(request);
                }

                return appointments.Count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
