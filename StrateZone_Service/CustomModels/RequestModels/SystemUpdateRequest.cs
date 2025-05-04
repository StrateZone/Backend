using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class SystemUpdateRequest
    {
        public TimeOnly OpenTime { get; set; }

        public TimeOnly CloseTime { get; set; }

        public decimal Appointment_Refund100_HoursFromScheduleTime { get; set; }

        public decimal Appointment_Incoming_HoursFromScheduleTime { get; set; }

        public int Appointment_Checkin_MinutesFromScheduleTime { get; set; }

        public int Max_NumberOfTables_CancelPerWeek { get; set; }

        public int ContributionPoints_PerThread { get; set; }

        public int ContributionPoints_PerComment { get; set; }

        public float UserPoints_PerCheckinTable_ByPercentageOfTablesPrice { get; set; }

        public int Numberof_TopContributors_PerWeek { get; set; }

        public int Max_NumberOfUsers_InviteToTable { get; set; }

        public float AppointmentRequest_MaxHours_UntilExpiration { get; set; }

        public float AppointmentRequest_MinHours_UntilExpiration { get; set; }

        public string Status { get; set; }
    }
}
