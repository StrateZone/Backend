using AutoMapper;
using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Entities;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StrateZone_Service.Implements
{
    public class SystemService : ISystemService
    {
        private readonly ISystemRepository _systemRepository;
        private readonly IMapper _mapper;

        public SystemService(ISystemRepository systemRepository, IMapper mapper)
        {
            _systemRepository = systemRepository;
            _mapper = mapper;
        }

        public async Task<AbnormalDayModel> AddAbnormalDayAsync(AbnormalDayRequest request)
        {
            try
            {
                var model = new AbnormalDayModel()
                {
                    SystemId = request.SystemId,
                    OpenTime = request.OpenTime,
                    CloseTime = request.CloseTime,
                    Date = request.Date,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified),
                };

                var day = _mapper.Map<AbnormalDay>(model);
                var result = await _systemRepository.AddAbnormalDayAsync(day);

                return _mapper.Map<AbnormalDayModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<AbnormalDayModel> DeleteAbnormalDayAsync(int id)
        {
            try
            {
                var result = await _systemRepository.DeleteAbnormalDayAsync(id);
                return _mapper.Map<AbnormalDayModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<AbnormalDayModel>> GetAbnormalDaysAsync(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var response = await _systemRepository.GetAbnormalDaysAsync(id, parameters);
                var mapped = _mapper.Map<PagedList<AbnormalDayModel>>(response);

                return new PagedList<AbnormalDayModel>(mapped, response.TotalCount, response.CurrentPage, response.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> GetAppointmentCheckinTimeInMinuesAsync(int id)
        {
            try
            {
                return await _systemRepository.GetAppointmentCheckinTimeInMinuesAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<decimal> GetAppointmentIncomingTimeInHoursAsync(int id)
        {
            try
            {
                return await _systemRepository.GetAppointmentIncomingTimeInHoursAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<decimal> GetAppointmentRefund100TimeInHoursAsync(int id)
        {
            try
            {
                return await _systemRepository.GetAppointmentRefund100TimeInHoursAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetClosingHourAsync(int id)
        {
            try
            {
                return await _systemRepository.GetClosingHourAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetClosingHourOnDateAsync(int id, DateOnly date)
        {
            try
            {
                return await _systemRepository.GetClosingHourOnDateAsync(id, date);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetOpeningHourAsync(int id)
        {
            try
            {
                return await _systemRepository.GetOpeningHourAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TimeOnly> GetOpeningHourOnDateAsync(int id, DateOnly date)
        {
            try
            {
                return await _systemRepository.GetOpeningHourOnDateAsync(id, date);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<List<SystemModel>> GetSystemsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<SystemModel> GetSystemsByIdAsync(int id)
        {
            try
            {
                var system = await _systemRepository.GetSystemsByIdAsync(id);
                return _mapper.Map<SystemModel>(system);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<AbnormalDayModel> UpdateAbnormalDayAsync(AbnormalDayModel abnormalDayModel, int id)
        {
            try
            {
                var abnormalDay = _mapper.Map<AbnormalDay>(abnormalDayModel);
                var result = await _systemRepository.UpdateAbnormalDayAsync(abnormalDay, id);

                return _mapper.Map<AbnormalDayModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<SystemModel> UpdateAppointmentTimeRulesAsync(int id, decimal refund100Time, decimal incomingTime, int minutesCheckin)
        {
            try
            {
                var result = await _systemRepository.UpdateAppointmentTimeRulesAsync(id, refund100Time, incomingTime, minutesCheckin);

                return _mapper.Map<SystemModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<SystemModel> UpdateSystemWorkingTimeAsync(int id, TimeOnly openTime, TimeOnly closeTime)
        {
            try
            {
                var result = await _systemRepository.UpdateSystemWorkingHoursAsync(id, openTime, closeTime);
            
                return _mapper.Map<SystemModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
