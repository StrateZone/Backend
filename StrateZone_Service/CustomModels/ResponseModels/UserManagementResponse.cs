using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class UserManagementResponse
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
        public string? UserLabel { get; set; }

        public string? Status { get; set; }
        public string? UserRole { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
