using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class OpponentResponse
    {
        public int UserId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string Status { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public int? Points { get; set; }

        public string UserRole { get; set; }

        public string Gender { get; set; }

        public string Ranking { get; set; }

        public string UserLabel { get; set; }

        public int ContributionPoints { get; set; }
    }
}
