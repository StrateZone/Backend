using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? OrderId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? User { get; set; }
}
