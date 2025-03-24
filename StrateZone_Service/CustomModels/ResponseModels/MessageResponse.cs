using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class MessageResponse
    {
        public int MessageId { get; set; }

        public int? SenderId { get; set; }

        public string? SenderName { get; set; }

        public string? SenderAvatar { get; set; }

        public int? ReceiverId { get; set; }
        
        public string? ReceiverName { get; set; }

        public string? ReceiverAvatar { get; set; }

        public string? Content { get; set; }

        public MessageStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Utc);
    }
}
