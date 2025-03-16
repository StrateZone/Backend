using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class FriendrequestModel
    {
        public int Id { get; set; }

        public int FromUser { get; set; }

        public int ToUser { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual UserModel FromUserNavigation { get; set; } = null!;

        public virtual UserModel ToUserNavigation { get; set; } = null!;
    }
}
