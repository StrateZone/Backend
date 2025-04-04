using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class AbnormalDayRequest
    {
        [Required]
        public int SystemId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly OpenTime { get; set; }

        [Required]
        public TimeOnly CloseTime { get; set; }
    }
}
