using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Entities
{
    public class System
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

        public TimeOnly OpenTime { get; set; }

        public TimeOnly CloseTime { get; set; }

        public decimal Appointment_Refund100_HoursFromScheduleTime { get; set; }

        public decimal Appointment_Incoming_HoursFromScheduleTime { get; set; }

        public int Appointment_Checkin_MinutesFromScheduleTime { get; set; }

        public string Status { get; set; }

        public virtual User? User { get; set; }
    
        public virtual ICollection<AbnormalDay> AbnormalDays { get; set; } = new List<AbnormalDay>();

        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
