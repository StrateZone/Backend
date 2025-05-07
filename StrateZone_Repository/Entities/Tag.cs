using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Tag
{
    public int TagId { get; set; }

    public string? TagName { get; set; }

    public string TagColor { get; set; }

    public TagStatus Status { get; set; } = TagStatus.active;

    public UserRole AllowedRole { get; set; } = UserRole.Member;

    public virtual ICollection<ThreadsTag> ThreadsTags { get; set; } = new List<ThreadsTag>();
}
