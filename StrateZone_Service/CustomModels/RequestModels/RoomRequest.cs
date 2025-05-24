using StrateZone_Service.BusinessModels;
using System.ComponentModel.DataAnnotations;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class RoomRequest
    {
        [StringLength(5)]
        [Required]
        public string? RoomName { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public int? Capacity { get; set; }

        [Required]
        public RoomStatus Status { get; set; } = RoomStatus.available;

        [Required]
        public bool IsForMonthlyBooking { get; set; } = false;
    }
}
