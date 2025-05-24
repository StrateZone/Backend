using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public AppointmentStatus Status { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsMonthlyAppointment { get; set; }

    public virtual ICollection<TablesAppointment> TablesAppointments { get; set; } = new List<TablesAppointment>();

    public virtual ICollection<Appointmentrequest> Appointmentrequests { get; set; } = new List<Appointmentrequest>();

    public virtual User? User { get; set; }
}
