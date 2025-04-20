using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Table
{
    public int TableId { get; set; }

    public int? RoomId { get; set; }

    public int? GameTypeId { get; set; }

    public TableStatus Status { get; set; } = TableStatus.active;

    public virtual GameType? GameType { get; set; }

    public virtual Room? Room { get; set; }

    public virtual ICollection<TablesAppointment> TablesAppointments { get; set; } = new List<TablesAppointment>();

    public virtual ICollection<Appointmentrequest> Appointmentrequests { get; set; } = new List<Appointmentrequest>();
}
