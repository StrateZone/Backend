using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class TransactionResponse
    {
        public string Month { get; set; }

        public decimal Booking { get; set; }

        public decimal MemberShip { get; set; }

        public decimal Spending { get; set; }

        public decimal Refund { get; set; }

        public decimal Voucher { get; set; }
    }
}
