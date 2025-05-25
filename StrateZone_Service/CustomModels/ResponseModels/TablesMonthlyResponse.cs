using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class TablesMonthlyResponse
    {
        public string DayOfWeek { get; set; }

        public Dictionary<DateOnly, TableResponse> DatesAndTables { get; set; } = new();
    }
}
