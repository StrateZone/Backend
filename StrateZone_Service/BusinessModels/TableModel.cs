using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class TableModel
    {
        public int TableId { get; set; }

        public int? RoomId { get; set; }

        public int? GameTypeId { get; set; }
        public string? Status { get; set; }

        //public virtual GameTypeModel? GameType { get; set; }

        // public virtual RoomModel? Room { get; set; }
    }
}
