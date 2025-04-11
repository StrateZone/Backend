using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class CommentRequest
    {
        public int? ReplyTo { get; set; }

        public int? ThreadId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string? Content { get; set; }
    }
}
