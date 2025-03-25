using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
        
        public int ToUser { get; set; }

        public MessageStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public User ToUserNavigation { get;set; }
    }
}
