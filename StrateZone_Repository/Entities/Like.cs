using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Like
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? CommentId { get; set; }

    public int? ThreadId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual Thread? Thread { get; set; }

    public virtual User? User { get; set; }
}
