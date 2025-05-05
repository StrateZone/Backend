using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string? VoucherName { get; set; }

    public int Value { get; set; }

    public bool IsSample { get; set; }

    public int? UserId { get; set; }

    public string? Description { get; set; }

    public decimal? MinPriceCondition { get; set; }

    public int? PointsCost { get; set; }

    public int? ContributionPointsCost { get; set; }

    public DateOnly? DayOfUsage { get; set; }

    public VoucherStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
