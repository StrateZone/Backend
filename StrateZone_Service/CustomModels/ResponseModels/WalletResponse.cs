using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class WalletResponse
    {
        public int WalletId { get; set; }

        public int? UserId { get; set; }

        public decimal? Balance { get; set; }

        public string Status { get; set; }
    }
}
