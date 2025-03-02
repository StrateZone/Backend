using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public partial class AppointmentModel
    {
        public int AppointmentId { get; set; }

        public DateTime ScheduleTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<TablesAppointment> TablesAppointments { get; set; } = new List<TablesAppointment>();

        public virtual UserModel? User { get; set; }
    }
}
