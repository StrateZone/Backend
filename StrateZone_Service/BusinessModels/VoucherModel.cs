using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class VoucherModel
    {
        public int VoucherId { get; set; }

        public string? VoucherName { get; set; }

        public int Value { get; set; }

        public string? Description { get; set; }

        public decimal? MinPriceCondition { get; set; }

        public DateOnly? ExpireDate { get; set; }

        public VoucherStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; } = new List<PaymentModel>();
    }
}
