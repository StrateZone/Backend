using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class UnavailableTableErrorResponse
    {
        public int TableId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
