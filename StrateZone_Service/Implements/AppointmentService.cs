using AutoMapper;
using Azure.Core;
using MealHunt_Repositories.Pagination;
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
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, IUserService userService, ITableService tableService, ITablesAppointmentService tablesAppointmentService, IPriceService priceService, IMapper mapper, IAppointmentrequestService appointmentrequestService)
        {
            _appointmentRepository = appointmentRepository;
            _userService = userService;
            _mapper = mapper;
            _tableService = tableService;
            _priceService = priceService;
            _tablesAppointmentService = tablesAppointmentService;
            _appointmentrequestService = appointmentrequestService;
        }

        public async Task<PagedList<AppointmentModel>> GetAppointmentsAsync(AppointmentParameters parameters)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentsAsync(parameters);
                var appointments = _mapper.Map<PagedList<AppointmentModel>>(result);

                return new PagedList<AppointmentModel>(appointments, appointments.TotalCount, appointments.CurrentPage, appointments.PageSize);
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

                return new PagedList<AppointmentModel>(appointments, appointments.TotalCount, appointments.CurrentPage, appointments.PageSize);
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

        public async Task<List<int>> CheckAppointmentAvailability(AppointmentRequest request)
        {
            List<TableResponse> Tables = await _tableService.GetAllTablesAsync();
            List<TableResponse> AvailableTables = await _tableService.GetAllAvailableTablesAsync(request.ScheduleTime, request.EndTime);

            var tableIds = new HashSet<int>(Tables.Select(t => t.TableId));
            var availableTableIds = new HashSet<int>(AvailableTables.Select(t => t.TableId));

            var unavailableTables = request.TableIds
                                        .Where(t => !tableIds.Contains(t) || !availableTableIds.Contains(t))
                                        .ToList();

            return unavailableTables;
        }

        public async Task<AppointmentModel> CreateAppointmentAsync(CustomModels.RequestModels.AppointmentRequest request)
        {
            try
            {
                List<int> unavailableTables = await CheckAppointmentAvailability(request);

                if (unavailableTables.Count > 0)
                {
                    throw new InvalidOperationException($"The following tables are not available: {string.Join(", ", unavailableTables)}");
                }

                AppointmentModel appointmentModel = new AppointmentModel()
                {
                    UserId = request.UserId,
                    ScheduleTime = request.ScheduleTime,
                    EndTime = request.EndTime,
                    TotalPrice = await _priceService.GetPriceOfAppointmentFromAppointmentRequestAsync(request.TableIds.ToArray(), request.ScheduleTime, request.EndTime),
                    CreatedAt = DateTime.UtcNow,
                };

                var mappedAppointment = _mapper.Map<Appointment>(appointmentModel);
                var appointment = await _appointmentRepository.CreateAppointmentAsync(mappedAppointment);
                var result = _mapper.Map<AppointmentModel>(appointment);

                foreach (var tableId in request.TableIds)
                {
                    TablesAppointmentModel tablesAppointmentModel = new TablesAppointmentModel()
                    {
                        TableId = tableId,
                        AppointmentId = result.AppointmentId
                    };

                    result.TablesAppointments.Add(tablesAppointmentModel);
                }

                var tablesAppointment = await _tablesAppointmentService.CreateTablesAppointmentsFromAppointmentAsync(result);
                result.TablesAppointments = tablesAppointment;

                var requests = await _appointmentrequestService.LinkAppointmentrequestsToAppointmentAsync(result);
                result.AppointmentrequestModels = requests;

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
