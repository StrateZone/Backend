using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class FriendlistModel
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? FriendId { get; set; }

        public virtual UserModel? Friend { get; set; }

        public virtual UserModel? User { get; set; }
    }
}
