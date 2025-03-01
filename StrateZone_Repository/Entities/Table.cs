using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Table
{
    public int TableId { get; set; }

    public int? RoomId { get; set; }

    public decimal? Fee { get; set; }

    public int? GameExtensionId { get; set; }

    public virtual GameExtension? GameExtension { get; set; }

    public virtual Room? Room { get; set; }

    public virtual ICollection<TablesAppointment> TablesAppointments { get; set; } = new List<TablesAppointment>();
}
