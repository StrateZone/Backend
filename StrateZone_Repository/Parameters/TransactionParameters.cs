using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Parameters
{
    public class TransactionParameters : PagedListParameters
    {
        public string? SearchValue { get; set; }
        public string? Type { get; set; } = "all"; // "all", "user", or "system"
    }
}
