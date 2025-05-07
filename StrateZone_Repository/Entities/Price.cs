using System;
using System.Collections;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Price
{
    public int Id { get; set; }

    public int? GameTypeId { get; set; }

    public int? ProductId { get; set; }

    public int? CourseId { get; set; }

    public bool MemberFee { get; set; }

    public bool TeachingSalary { get; set; }

    public string? RoomType { get; set; }

    public decimal? Price1 { get; set; }

    public string? Unit { get; set; }

    public string Type { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual GameType? GameType { get; set; }
}
