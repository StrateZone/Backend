using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Transaction
{
    public int Id { get; set; }

    public int? OfUser { get; set; }

    public string? ReferenceId { get; set; }

    public string? Content { get; set; }

    public decimal? Amount { get; set; }

    public TransactionType TransactionType { get; set; } 

    public DateTime? CreatedAt { get; set; }

    public virtual User? OfUserNavigation { get; set; }
}
