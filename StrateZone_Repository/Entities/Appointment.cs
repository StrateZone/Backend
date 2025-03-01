using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? UserId { get; set; }

    public DateTime? ScheduleTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TablesAppointment> TablesAppointments { get; set; } = new List<TablesAppointment>();

    public virtual User? User { get; set; }
}
