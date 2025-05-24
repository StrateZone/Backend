using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class AppointmentResponse
    {
        public int AppointmentId { get; set; }

        public int UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public int TablesCount { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UserResponse? User { get; set; }

        public bool IsMonthlyAppointment { get; set; }

        public virtual ICollection<TablesAppointmentResponse> TablesAppointments { get; set; } = [];

        public virtual ICollection<AppointmentrequestResponse>? Appointmentrequests { get; set; } = [];
    }
}
