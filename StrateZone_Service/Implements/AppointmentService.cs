using AutoMapper;
using StrateZone_Repository.Implements;
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
    public class AppointmentService 
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, IUserService userService, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<List<AppointmentModel>> GetAppointmentsAsync()
        {
            try
            {
                var result = await _appointmentRepository.GetAppointmentsAsync();
                return _mapper.Map<List<AppointmentModel>>(result);
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

        public async Task CreateAppointmentAsync(AppointmentRequest request)
        {
            try
            {
                AppointmentModel appointment = new AppointmentModel()
                {
                    User = await _userService.GetUserByIdAsync(request.UserId),
                    ScheduleTime = request.ScheduleTime,
                    EndTime = request.EndTime,
                    CreatedAt = DateTime.UtcNow
                };
                var result = _mapper.Map<AppointmentModel>(appointment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
