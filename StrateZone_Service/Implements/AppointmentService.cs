using AutoMapper;
using Azure.Core;
using MealHunt_Repositories.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
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
        private readonly IPriceService _priceService;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, IUserService userService, ITableService tableService, ITablesAppointmentService tablesAppointmentService, IPriceService priceService, IMapper mapper, IAppointmentrequestService appointmentrequestService, IEmailService emailService, IPaymentService paymentService)
        {
            _appointmentRepository = appointmentRepository;
            _userService = userService;
            _mapper = mapper;
            _tableService = tableService;
            _priceService = priceService;
            _tablesAppointmentService = tablesAppointmentService;
            _appointmentrequestService = appointmentrequestService;
            _emailService = emailService;
            _paymentService = paymentService;
        }

        public async Task<PagedList<AppointmentModel>> GetAppointmentsAsync(AppointmentParameters parameters)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentsAsync(parameters);
                var appointments = _mapper.Map<PagedList<AppointmentModel>>(result);

                return new PagedList<AppointmentModel>(appointments, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentResponse>> GetAllAppointmentsAsync(AppointmentParameters parameters)
        {
            try
            {
                var result = await _appointmentRepository.GetAllAppointmentsAsync(parameters);
                var appointments = _mapper.Map<PagedList<AppointmentResponse>>(result);

                return new PagedList<AppointmentResponse>(appointments, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<AppointmentModel>> GetAppointmentsByUserIdAsync(AppointmentParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentsByUserIdAsync(parameters, userId);
                var appointments = _mapper.Map<PagedList<AppointmentModel>>(result);

                return new PagedList<AppointmentModel>(appointments, result.TotalCount, result.CurrentPage, result.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppointmentModel> GetAppointmentByIdAsync(int id)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentByIdAsync(id);
                return _mapper.Map<AppointmentModel>(result);
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
                if (!ScheduleTimeValidator.IsScheduleTimeValid(ta.ScheduleTime, ta.EndTime, false, out string err))
                    throw new Exception(err);
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

    }
}
