using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class UsersCourse
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? UserId { get; set; }

    public UserCourseResult Result { get; set; }

    public DateTime? EnrolledAt { get; set; }

    public ParticipantStatus? ParticipantStatus { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? User { get; set; }
}
