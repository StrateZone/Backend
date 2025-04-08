using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class NotificationModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int ToUser { get; set; }

        public int? TablesAppointmentId { get; set; }

        public int? OrderId { get; set; }

        public int? TournamentId { get; set; }

        public MessageStatus Status { get; set; }

        public NotificationType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual UserModel? ToUserNavigation { get; set; }

        // public virtual OrderModel? Order { get; set; }

        // public virtual TablesAppointmentModel? TablesAppointment { get; set; }

        // public virtual TournamentModel? Tournament { get; set; }
    }
}
