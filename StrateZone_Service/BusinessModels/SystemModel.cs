using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class SystemModel
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

        public TimeOnly OpenTime { get; set; }

        public TimeOnly CloseTime { get; set; }

        public string Status { get; set; }

        // public virtual UserModel? User { get; set; }

        //public virtual ICollection<AbnormalDay> AbnormalDays { get; set; } = new List<AbnormalDay>();

        //public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
