using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class MessageModel
    {
        public int MessageId { get; set; }

        public int? SenderId { get; set; }

        public int? ReceiverId { get; set; }

        public string? Content { get; set; }

        public MessageStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc);

        public virtual UserModel? Receiver { get; set; }

        public virtual UserModel? Sender { get; set; }
    }
}
