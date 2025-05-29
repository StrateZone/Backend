using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class TablesAppointment
{
    public int Id { get; set; }

    public int? TableId { get; set; }

    public int? AppointmentId { get; set; }

    public decimal? Price { get; set; }

    public DateTime ScheduleTime { get; set; }

    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; } 

    public DateTime? CreatedAt { get; set; }

    public bool PaidForOpponent { get; set; }

    public string? Note { get; set; }

    public bool IsExtended { get; set; }

    public int? ExtendedOf { get; set; }

    public int ExtendedCount { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Table? Table { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
