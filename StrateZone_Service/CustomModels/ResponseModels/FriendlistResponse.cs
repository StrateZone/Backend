using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class FriendlistResponse
    {
        public int Id { get; set; }

        public int? FriendId { get; set; }

        public virtual UserResponse? Friend { get; set; }
    }
}
