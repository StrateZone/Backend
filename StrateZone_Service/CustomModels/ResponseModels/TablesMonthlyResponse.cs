using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class TablesMonthlyResponse
    {
        public int ExpectedTablesCount => DatesAndTables.Sum(t => t.Value.Count);
        public int ActualTablesCount => DatesAndTables.Sum(t => t.Value.Count(tr => tr != null));
        public Dictionary<DayOfWeek, List<TableDateResponse>> DatesAndTables { get; set; } = new();
    }

    public class TableDateResponse
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateOnly OnDate { get; set; }
        public TableResponse? TableResponse { get; set; }
    }
}
