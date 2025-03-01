using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Cart
{
    public int CartId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
