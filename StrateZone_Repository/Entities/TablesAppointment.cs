using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class TablesAppointment
{
    public int Id { get; set; }

    public int? TableId { get; set; }

    public int? AppointmentId { get; set; }

    public int? GameExtensionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual GameExtension? GameExtension { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Table? Table { get; set; }
}
