using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Message
{
    public int MessageId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User? Sender { get; set; }
}
