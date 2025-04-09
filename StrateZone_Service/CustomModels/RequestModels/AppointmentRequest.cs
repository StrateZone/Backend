using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class AppointmentRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<TablesAppointmentRequest> TablesAppointmentRequests { get; set; } = new List<TablesAppointmentRequest>();

        [Required]
        public decimal TotalPrice { get; set; }
    }
}


