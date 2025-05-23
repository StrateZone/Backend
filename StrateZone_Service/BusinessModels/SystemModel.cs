using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class SystemModel
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

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

        public int Max_NumberOfUsers_InvitedToTable { get; set; }

        public float AppointmentRequest_MaxHours_UntilExpiration { get; set; }

        public float AppointmentRequest_MinHours_UntilExpiration { get; set; }

        public float PercentageRefund_IfNot100 { get; set; }

        public float PercentageTimeRange_UntilRequestExpiration { get; set; }

        public int Verification_OTP_Duration { get; set; }

        public int Min_Minutes_For_TablesExtend { get; set; }

        public int Max_Minutes_For_TablesExtend { get; set; }

        public int ExtendAllow_BeforeMinutes_FromTableComplete { get; set; }

        public int ExtendCancel_BeforeMinutes_FromPlayTime { get; set; }

        public float Percentage_Refund_On_ExtendedTables { get; set; }

        public string Status { get; set; }

        // public virtual UserModel? User { get; set; }

        //public virtual ICollection<AbnormalDay> AbnormalDays { get; set; } = new List<AbnormalDay>();

        //public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
