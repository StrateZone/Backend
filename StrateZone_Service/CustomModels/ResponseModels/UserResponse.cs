using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class UserResponse
    {
        public int UserId { get; set; }

        public int? CartId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string UserRole { get; set; }

        public string? FullName { get; set; }

        public string Status { get; set; }

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public int? Points { get; set; }

        public string Gender { get; set; }

        public string SkillLevel { get; set; }

        public string Ranking { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Friendlist> FriendlistUsers { get; set; } = new List<Friendlist>();

        public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    }
}
