using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class CommentModel
    {
        public int CommentId { get; set; }

        public int? ReplyTo { get; set; }

        public int? ThreadId { get; set; }

        public int? UserId { get; set; }

        public string? Content { get; set; }

        public double? Rating { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<CommentModel> InverseReplyToNavigation { get; set; } = new List<CommentModel>();

        public virtual ICollection<LikeModel> Likes { get; set; } = new List<LikeModel>();

        public virtual CommentModel? ReplyToNavigation { get; set; }

        public virtual ThreadModel? Thread { get; set; }

        public virtual User? User { get; set; }
    }
}
