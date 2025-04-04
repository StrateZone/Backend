using Microsoft.AspNetCore.Mvc;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Parameters
{
    public class AppointmentAdminParameters : PagedListParameters
    {
        public AppointmentStatus? Status { get; set; }

        
        public string? SearchValue { get; set; }
    }
}
