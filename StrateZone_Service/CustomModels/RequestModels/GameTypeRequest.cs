using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class GameTypeRequest
    {
        public string TypeName { get; set; }

        public decimal PricePerHour { get; set; }
    }
}
