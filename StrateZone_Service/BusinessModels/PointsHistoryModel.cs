using StrateZone_Repository.Entities;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class PointsHistoryModel
    {
        public int Id { get; set; }

        public int? OfUser { get; set; }

        public string? Description { get; set; }

        public int? Amount { get; set; }

        public string? Content { get; set; }

        public string PointType { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
