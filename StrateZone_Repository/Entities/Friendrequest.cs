using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Friendrequest
{
    public int Id { get; set; }

    public int FromUser { get; set; }

    public int ToUser { get; set; }

    public RequestStatus Status { get; set; }
    
    public DateTime? CreatedAt { get; set; }

    public virtual User FromUserNavigation { get; set; } = null!;

    public virtual User ToUserNavigation { get; set; } = null!;
}
