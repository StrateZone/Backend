using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class RoomModel
    {
        public int RoomId { get; set; }

        public string? RoomName { get; set; }

        public RoomType Type { get; set; }

        public string? Description { get; set; }

        public int? Capacity { get; set; }

        public RoomStatus Status { get; set; }

        public virtual ICollection<TableModel> Tables { get; set; } = new List<TableModel>();
    }
}
