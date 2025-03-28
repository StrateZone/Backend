using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class AppointmentrequestModel
    {
        public int Id { get; set; }

        public int FromUser { get; set; }

        public int ToUser { get; set; }

        public int TableId { get; set; }

        public int? AppointmentId { get; set; } = null;

        public string Status { get; set; }
        
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? ExpireAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual UserModel FromUserNavigation { get; set; } = null!;

        public virtual UserModel ToUserNavigation { get; set; } = null!;

        public virtual TableModel Table { get; set; } = null!;

        public virtual TablesAppointmentModel? Appointment { get; set; }
    }
}
