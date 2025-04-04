using Azure.Core;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Utils
{
    public class ScheduleTimeValidator
    {
        private readonly ISystemService _systemService;

        public ScheduleTimeValidator(ISystemService systemService) 
        { 
            _systemService = systemService;
        }

        public async Task<(bool isValid, string errorMessage)> IsScheduleTimeValid(TableParameters request, bool softCheck)
        {
            return await IsScheduleTimeValid(request.StartTime, request.EndTime, softCheck);
        }

        public async Task<(bool isValid, string errorMessage)> IsScheduleTimeValid(TableParameters request)
        {
            return await IsScheduleTimeValid(request.StartTime, request.EndTime, false);
        }

        public async Task<(bool isValid, string errorMessage)> IsScheduleTimeValid(TablesAppointmentModel request)
        {
            return await IsScheduleTimeValid(request.ScheduleTime, request.EndTime, false);
        }

        public async Task<(bool isValid, string errorMessage)> IsScheduleTimeValid(DateTime scheduleTime, DateTime endTime, bool softCheck)
        {
            string errorMessage = string.Empty;

            if (scheduleTime <= DateTime.UtcNow.AddHours(7)
                || endTime <= DateTime.UtcNow.AddHours(7))
            {
                errorMessage = "Can not select time in the past.";
                return (false, errorMessage);
            }

            if (scheduleTime > endTime)
            {
                errorMessage = "Start time must be earlier than End time.";
                return (false, errorMessage);
            }

            if (endTime.Subtract(scheduleTime).TotalHours < 0.5)
            {
                errorMessage = "The minimum duration between start and end time is 30 minutes.";
                return (false, errorMessage);
            }

            if (softCheck) return (true, errorMessage);

            if (scheduleTime.Date != endTime.Date)
            {
                errorMessage = "Start time and End time must be within the same day.";
                return (false, errorMessage);
            }

            DateOnly ScheduleDatePart = DateOnly.FromDateTime(scheduleTime),
                     EndDatePart = DateOnly.FromDateTime(endTime);

            TimeOnly OpenHour = await _systemService.GetOpeningHourOnDateAsync(1, ScheduleDatePart);
            TimeOnly CloseHour = await _systemService.GetClosingHourOnDateAsync(1, EndDatePart);

            TimeOnly ScheduleTimePart = TimeOnly.FromDateTime(scheduleTime);
            TimeOnly EndTimePart = TimeOnly.FromDateTime(endTime);

            if (ScheduleTimePart < OpenHour || EndTimePart > CloseHour ||
                (EndTimePart == CloseHour && endTime.Minute > 0))
            {
                errorMessage = $"Appointment on date {ScheduleDatePart} must be scheduled between {OpenHour} and {CloseHour}.";
                return (false, errorMessage);
            }

            if (scheduleTime.Minute % 30 != 0 || endTime.Minute % 30 != 0)
            {
                errorMessage = "Appointment's schedule and end time's minute parts must be divisible by 30.";
                return (false, errorMessage);
            }

            return (true, errorMessage);
        }
    }
}
