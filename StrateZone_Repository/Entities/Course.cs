using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Course
{
    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    public string? Description { get; set; }

    public int? InstructorId { get; set; }

    public Parameters.PostgreEnums.GameTypeEnum GameType { get; set; }

    public SkillLevel SkillLevel { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? MaxParticipants { get; set; }

    public CourseStatus CourseStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CoursesSlot> CoursesSlots { get; set; } = new List<CoursesSlot>();

    public virtual User? Instructor { get; set; }

    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

    public virtual ICollection<UsersCourse> UsersCourses { get; set; } = new List<UsersCourse>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
