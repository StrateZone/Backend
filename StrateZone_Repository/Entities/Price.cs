using System;
using System.Collections;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Price
{
    public int Id { get; set; }

    public int? GameTypeId { get; set; }

    public int? ProductId { get; set; }

    public int? CourseId { get; set; }

    public BitArray? MemberFee { get; set; }

    public BitArray? TeachingSalary { get; set; }

    public decimal? Price1 { get; set; }

    public string? Unit { get; set; }

    public virtual Course? Course { get; set; }

    public virtual GameType? GameType { get; set; }

    public virtual Product? Product { get; set; }
}
