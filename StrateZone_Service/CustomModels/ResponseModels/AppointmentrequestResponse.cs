using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class AppointmentrequestResponse
    {
        public int Id { get; set; }

        public int ToUser { get; set; }

        public string Status { get; set; }

        public int TableId { get; set; }

        public int AppointmentId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? ExpireAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual UserResponse ToUserNavigation { get; set; } = null!;
    }
}
