using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Image
{
    public int ImageId { get; set; }

    public int? UserId { get; set; }
    
    public int? ProductId { get; set; }

    public int? ThreadId { get; set; }

    public int? GameTypeId { get; set; }

    public int? TournamentId { get; set; }

    public int? EventId { get; set; }

    public string? Url { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }

    public virtual GameType? GameType { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual Event? Event { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Thread? Thread { get; set; }
}
