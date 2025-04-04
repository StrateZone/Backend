using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Entities
{
    public class AbnormalDay
    {
        public int Id { get; set; }

        public int SystemId { get; set; }

        public DateOnly Date {  get; set; }

        public TimeOnly OpenTime { get; set; }

        public TimeOnly CloseTime { get; set; }
    
        public DateTime? CreatedAt { get; set; }
    
        public virtual System? System { get; set; }
    }
}
