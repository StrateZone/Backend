using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class LikeModel
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? CommentId { get; set; }

        public int? ThreadId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual CommentModel? Comment { get; set; }

        public virtual ThreadModel? Thread { get; set; }

        public virtual UserModel? User { get; set; }
    }
}
