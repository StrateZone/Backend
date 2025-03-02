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
        public int UserId;

        [Required]
        public int GameExtensionId;

        [Required]
        public DateTime ScheduleTime;

        [Required]
        public DateTime EndTime;

        [Required]
        [MinLength(1)]
        public List<int> TableIds = new List<int>();
    }
}
