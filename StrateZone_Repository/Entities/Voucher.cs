using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string? VoucherName { get; set; }

    public string? Description { get; set; }

    public decimal? MinPriceCondition { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
