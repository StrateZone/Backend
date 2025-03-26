using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public partial class AppointmentModel
    {
        public int AppointmentId { get; set; }

        public int UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UserModel? User { get; set; }

        public virtual ICollection<TablesAppointmentModel> TablesAppointments { get; set; } = [];
    
        public virtual ICollection<AppointmentrequestModel>? Appointmentrequests { get; set; } = [];
    }
}
