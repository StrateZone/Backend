using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class NotificationRequest
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int ToUser { get; set; }

        public int? TablesAppointmentId { get; set; }

        public int? OrderId { get; set; }

        public int? TournamentId { get; set; }
    }
}
