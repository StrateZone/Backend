using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public int? InventoryCount { get; set; }

    public string? ImageUrl { get; set; }

    public ProductStatus Status { get; set; }
    
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}
