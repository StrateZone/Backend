using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class BookingPaymentRequest
    {
        public int UserId { get; set; }
        public List<TableBookingPaymentRequest> Tables { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
