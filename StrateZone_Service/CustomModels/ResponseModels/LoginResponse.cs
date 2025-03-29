using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class LoginResponse
    {
        public int UserId { get; set; }

        public string UserRole { get; set; }

        public string Status { get; set; } = null!;

        //public int? CartId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Gender { get; set; }

        public string? SkillLevel { get; set; }

        public string? Ranking { get; set; }

        public string? FullName { get; set; }

        public string? Address { get; set; }

        public string? Bio { get; set; }

        public string? ImageUrl { get; set; }

        public WalletResponse? Wallet { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }
    }
}
