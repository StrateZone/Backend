using Azure.Core;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Utils
{
    public class ScheduleTimeValidator
    {
        public static bool IsScheduleTimeValid(TableParameters request, bool softCheck, out string errorMessage)
        {
            return IsScheduleTimeValid(request.StartTime, request.EndTime, softCheck, out errorMessage);
        }

        public static bool IsScheduleTimeValid(TableParameters request, out string errorMessage)
        {
            return IsScheduleTimeValid(request.StartTime, request.EndTime, false, out errorMessage);
        }

        public static bool IsScheduleTimeValid(TablesAppointmentModel request, out string errorMessage)
        {
            return IsScheduleTimeValid(request.ScheduleTime, request.EndTime, false, out errorMessage);
        }

        public static bool IsScheduleTimeValid(DateTime scheduleTime, DateTime endTime, bool softCheck, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (scheduleTime <= DateTime.UtcNow.AddHours(7)
                || endTime <= DateTime.UtcNow.AddHours(7))
            {
                errorMessage = "Can not select time in the past.";
                return false;
            }

            if (scheduleTime > endTime)
            {
                errorMessage = "Start time must be earlier than End time.";
                return false;
            }

            if (endTime.Subtract(scheduleTime).TotalHours < 0.5)
            {
                errorMessage = "The minimum duration between start and end time is 30 minutes.";
                return false;
            }

            if (softCheck) return true;

            if (scheduleTime.Date != endTime.Date)
            {
                errorMessage = "Start time and End time must be within the same day.";
                return false;
            }

            if (scheduleTime.Hour < 8 || endTime.Hour > 22 ||
                (endTime.Hour == 22 && endTime.Minute > 0))
            {
                errorMessage = "Appointment must be scheduled between 8AM and 10PM.";
                return false;
            }

            if (scheduleTime.Minute % 30 != 0 || endTime.Minute % 30 != 0)
            {
                errorMessage = "Appointment's schedule and end time's minute parts must be divisible by 30.";
                return false;
            }

            return true;
        }
    }
}
