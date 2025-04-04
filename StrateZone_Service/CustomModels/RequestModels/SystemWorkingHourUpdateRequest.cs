using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class SystemWorkingHourUpdateRequest
    {
        public TimeOnly OpenHour { get; set; }
        public TimeOnly CloseHour {  get; set; }
    }
}
