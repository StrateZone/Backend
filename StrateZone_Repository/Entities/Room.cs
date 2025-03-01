using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public string? RoomName { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<CoursesSlot> CoursesSlots { get; set; } = new List<CoursesSlot>();

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
