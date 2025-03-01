using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Event
{
    public int EventId { get; set; }

    public int? UserId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
