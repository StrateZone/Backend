using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Image
{
    public int ImageId { get; set; }

    public int? ProductId { get; set; }

    public int? ThreadId { get; set; }

    public string? Url { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Thread? Thread { get; set; }
}
