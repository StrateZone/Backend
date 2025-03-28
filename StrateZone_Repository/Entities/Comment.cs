using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? ReplyTo { get; set; }

    public int? ThreadId { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public double? Rating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> InverseReplyToNavigation { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual Comment? ReplyToNavigation { get; set; }

    public virtual Thread? Thread { get; set; }

    public virtual User? User { get; set; }
}
