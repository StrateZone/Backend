using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class ThreadModel
    {
        public int ThreadId { get; set; }

        public int? CreatedBy { get; set; }

        public string? Title { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? Content { get; set; }

        public double? Rating { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();

        public virtual UserModel? CreatedByNavigation { get; set; }

        public virtual ICollection<ImageModel> Images { get; set; } = new List<ImageModel>();

        public virtual ICollection<LikeModel> Likes { get; set; } = new List<LikeModel>();

        public virtual ICollection<ThreadsTagModel> ThreadsTags { get; set; } = new List<ThreadsTagModel>();
    }
}
