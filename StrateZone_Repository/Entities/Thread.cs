using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Thread
{
    public int ThreadId { get; set; }

    public int? CreatedBy { get; set; }

    public string? Title { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? Content { get; set; }

    public double? Rating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<ThreadsTag> ThreadsTags { get; set; } = new List<ThreadsTag>();
}
