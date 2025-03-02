using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Ticket
{
    public int Id { get; set; }

    public int? SenderId { get; set; }

    public string? Reason { get; set; }

    public TicketType TicketType { get; set; } 

    public DateTime? SentAt { get; set; }

    public string? AttachmentUrl { get; set; }

    public virtual User? Sender { get; set; }
}
