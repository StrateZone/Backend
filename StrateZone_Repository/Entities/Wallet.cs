using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Wallet
{
    public int WalletId { get; set; }

    public int? UserId { get; set; }

    public decimal? Balance { get; set; }

    public virtual User? User { get; set; }
}
