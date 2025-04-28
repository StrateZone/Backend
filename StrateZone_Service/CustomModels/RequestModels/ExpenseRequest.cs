using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class ExpenseRequest
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
