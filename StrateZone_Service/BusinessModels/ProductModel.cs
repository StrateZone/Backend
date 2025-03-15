using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public int? InventoryCount { get; set; }

        public string? ImageUrl { get; set; }

        public ProductStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        //public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        //public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual ICollection<PriceModel> Prices { get; set; } = new List<PriceModel>();

        //public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}
