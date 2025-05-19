using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class TablesAppointmentPaymentRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int OldTablesAppointmentId { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
