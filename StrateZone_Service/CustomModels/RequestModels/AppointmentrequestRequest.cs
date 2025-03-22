using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class AppointmentrequestRequest
    {
        [Required]
        public int FromUser { get; set; }

        [Required]
        public int ToUser { get; set; }

        [Required]
        public int TablesAppointmentId { get; set; }
    }
}
