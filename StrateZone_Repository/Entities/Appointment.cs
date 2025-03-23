using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public DateTime ScheduleTime { get; set; }

    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TablesAppointment> TablesAppointments { get; set; } = new List<TablesAppointment>();

    public virtual ICollection<Appointmentrequest> Appointmentrequests { get; set; } = new List<Appointmentrequest>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
