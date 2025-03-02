using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class UserModel
    {
        public int UserId { get; set; }

        public int? CartId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        /// <summary>
        /// Depends on Role
        /// </summary>
        public string Status { get; set; } = null!;

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public int? Points { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Friendlist> FriendlistUsers { get; set; } = new List<Friendlist>();

        public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    }
}
