using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class PaymentModel
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string PaymentType { get; set; }

        public string PaymentStatus { get; set; }

        public int? OrderId { get; set; }

        public int? TablesAppointmentId { get; set; }

        public int? CourseId { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual UserModel? User { get; set; }

        // public virtual Order? Order { get; set; }

        public virtual TablesAppointmentModel? TablesAppointment { get; set; }
    }
}
