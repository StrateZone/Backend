using StrateZone_Repository.Entities;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public partial class AppointmentModel
    {
        public int AppointmentId { get; set; }

        public int UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UserResponse? User { get; set; }

        public int TablesCount { get; set; }

        public bool IsMonthlyAppointment { get; set; }

        public virtual ICollection<TablesAppointmentModel> TablesAppointments { get; set; } = [];
    
        public virtual ICollection<AppointmentrequestModel>? Appointmentrequests { get; set; } = [];
    }
}
