using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Entities
{
    public class Expense
    {
        public int Id { get; set; } 

        public int UserId { get; set; }

        public int SystemId { get;set; }

        public decimal Amount {  get; set; }
    
        public string Type { get; set; }

        public string Description { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual User? User { get; set; }

        public virtual System? System { get; set; }
    }
}
