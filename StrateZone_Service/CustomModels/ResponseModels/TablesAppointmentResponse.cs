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

        public double DurationInHours => EndTime.Subtract(ScheduleTime).TotalHours;

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual TableResponse? Table { get; set; }
    }
}
