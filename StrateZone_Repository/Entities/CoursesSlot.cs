using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class CoursesSlot
{
    public int Id { get; set; }

    public int? RoomId { get; set; }

    public int? CourseId { get; set; }

    public int? InstructorId { get; set; }

    public DateTime? OnDate { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? Instructor { get; set; }

    public virtual Room? Room { get; set; }
}
