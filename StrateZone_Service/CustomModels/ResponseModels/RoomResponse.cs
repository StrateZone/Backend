using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class RoomResponse
    {
        public int RoomId { get; set; }

        public string? RoomName { get; set; }

        public string? Type { get; set; }

        public string? Description { get; set; }

        public int? Capacity { get; set; }

        public string? Status { get; set; }

        public decimal? Price { get; set; } = 0;

        public string? Unit { get; set; }

        public bool IsForMonthlyBooking { get; set; }

        public virtual ICollection<TableModel> Tables { get; set; } = new List<TableModel>();
    }
}
