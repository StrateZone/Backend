using Microsoft.AspNetCore.Builder;
using StrateZone_Service.BusinessModels;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class TableResponse
    {
        public int TableId { get; set; }

        public int? RoomId { get; set; }

        public string RoomName { get; set; }

        public string? RoomType { get; set; }

        public string? RoomDescription { get; set; }

        public int? GameTypeId { get; set; }

        public virtual GameTypeModel? GameType { get; set; }

        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }

        public decimal? GameTypePrice { get; set; }

        public decimal? RoomTypePrice { get; set; }
        
        public float? DurationInHours { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
