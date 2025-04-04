using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class AbnormalDayModel
    {
        public int Id { get; set; }

        public int SystemId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly OpenTime { get; set; }

        public TimeOnly CloseTime { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        public virtual SystemModel? System { get; set; }
    }
}
