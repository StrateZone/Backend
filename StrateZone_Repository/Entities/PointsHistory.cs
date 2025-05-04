using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities
{
    public class PointsHistory
    {
        public int Id { get; set; }

        public int? OfUser { get; set; }

        public string? Description { get; set; }

        public int? Amount { get; set; }

        public string? Content { get; set; }

        public string PointType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual User? OfUserNavigation { get; set; }
    }
}
