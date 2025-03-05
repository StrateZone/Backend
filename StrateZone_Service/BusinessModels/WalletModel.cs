using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class WalletModel
    {
        public int WalletId { get; set; }

        public int? UserId { get; set; }

        public decimal? Balance { get; set; }

        public WalletStatus Status { get; set; }

        public virtual UserModel? User { get; set; }
    }
}
