using Microsoft.AspNetCore.Mvc;

namespace StrateZone_Repository.Parameters
{
    public class TableParameters : PagedListParameters
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
