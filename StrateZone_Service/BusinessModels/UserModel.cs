using StrateZone_Repository.Entities;
using System.Text.Json.Serialization;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class UserModel
    {
        public int UserId { get; set; }

        public int? CartId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
        
        public string? FullName { get; set; }

        public string? Password { get; set; }

        public string? Status { get; set; } = "Active";

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public int? Points { get; set; }

        public int? ContributionPoints { get; set; }

        public UserLabel UserLabel { get; set; }

        public StrateZone_Repository.Parameters.PostgreEnums.UserRole UserRole { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.UserRole.RegisteredUser;

        public DateTime? MembershipExpiry { get; set; }

        public StrateZone_Repository.Parameters.PostgreEnums.Gender Gender { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.Gender.male;

        public StrateZone_Repository.Parameters.PostgreEnums.SkillLevel SkillLevel { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.SkillLevel.beginner;

        public StrateZone_Repository.Parameters.PostgreEnums.Ranking Ranking { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.Ranking.basic;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual WalletModel? Wallet { get; set; }
    }
}
