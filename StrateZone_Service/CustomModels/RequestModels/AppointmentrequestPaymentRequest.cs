using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class AppointmentrequestPaymentRequest
    {
        [Required]
        public int FromUser { get; set; }

        [Required]
        public int ToUser { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public int AppointmentId { get; set; }
    }
}
