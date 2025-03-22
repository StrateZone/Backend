using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class AppointmentRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime ScheduleTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [MinLength(1)]
        public List<int> TableIds { get; set; } = new List<int>();

        [Required]
        public decimal TotalPrice { get; set; }
    }
}
