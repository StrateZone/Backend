using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class LikeRequest
    {
        public int? UserId { get; set; }

        public int? CommentId { get; set; }

        public int? ThreadId { get; set; }
    }
}
