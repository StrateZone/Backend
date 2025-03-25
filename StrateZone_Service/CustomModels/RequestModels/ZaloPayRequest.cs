using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class ZaloPayRequest
    {
        public int UserId { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
    }
}
