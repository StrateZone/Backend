using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.DTO
{
    public class ThreadDTO
    {
        public int ThreadId { get; set; }

        public int? CreatedBy { get; set; }

        public string? Title { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? Content { get; set; }

        public double? Rating { get; set; }

        public int? LikesCount { get; set; }

        public int? CommentsCount { get; set; }

        public ThreadStatus Status { get; set; }

        public int? UpdateOfThread { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User? CreatedByNavigation { get; set; }

        public virtual ICollection<ThreadsTag> ThreadsTags { get; set; } = new List<ThreadsTag>();
    }
}
