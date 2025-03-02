using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public int? VoucherId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? TrackingNumber { get; set; }

    public decimal? TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
