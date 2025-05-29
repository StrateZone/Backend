using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class TablesAppointmentResponse
    {
        public int Id { get; set; }

        public int? TableId { get; set; }

        public int? AppointmentId { get; set; }

        public string Status { get; set; }

        public DateTime ScheduleTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool PaidForOpponent { get; set; }

        public double DurationInHours => EndTime.Subtract(ScheduleTime).TotalHours;

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Note { get; set; }

        public bool IsExtended { get; set; }

        public int? ExtendedOf { get; set; }

        public int ExtendedCount { get; set; }

        public bool AllowExtend { get; set; }

        public virtual TableResponse? Table { get; set; }
    }

    public class TablesAppointmentExtendResponse
    {
        public int Id { get; set; }

        public int? OldId { get; set; }

        public DateTime ScheduleTime { get; set; }

        public DateTime EndTime { get; set; }

        public int NumberOfExtends { get; set; }

        public int MaxNumberOfExtends { get; set; }

        public double DurationInHours => EndTime.Subtract(ScheduleTime).TotalHours;

        public string? Note { get; set; }

        public virtual TableResponse? Table { get; set; }
    }
}
