using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Parameters
{
    public class ThreadParameters : PagedListParameters
    {
        public string Search { get; set; } = string.Empty;
    
        public HashSet<int> TagIds {  get; set; } = new HashSet<int>();

        public PostgreEnums.ThreadStatus[] statuses { get; set; } = [];

        public int? userId { get; set; } = null;
    }
}
