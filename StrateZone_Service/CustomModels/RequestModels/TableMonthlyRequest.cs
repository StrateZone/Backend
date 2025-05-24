using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class TableMonthlyRequest
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public DayOfWeek dayOfWeek { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public string RoomType { get; set; }

        [Required]
        public string GameType { get; set; }
    }
}
