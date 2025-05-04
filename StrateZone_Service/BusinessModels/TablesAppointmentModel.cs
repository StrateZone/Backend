using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class TablesAppointmentModel
    {
        public int Id { get; set; }

        public int? TableId { get; set; }

        public int? AppointmentId { get; set; }

        public string Status { get; set; }

        public DateTime ScheduleTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual TableModel? Table { get; set; }


        // public virtual AppointmentModel? Appointment { get; set; }
    }
}
