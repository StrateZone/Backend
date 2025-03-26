using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class TablesAppointmentRequest
    {
        [Required]
        public decimal Price { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public DateTime ScheduleTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
