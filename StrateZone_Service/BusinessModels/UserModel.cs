using StrateZone_Repository.Entities;

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

        public string Status { get; set; } = "Active";

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public int? Points { get; set; }

        public StrateZone_Repository.Parameters.PostgreEnums.UserRole UserRole { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.UserRole.RegisteredUser;

        public StrateZone_Repository.Parameters.PostgreEnums.Gender Gender { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.Gender.male;

        public StrateZone_Repository.Parameters.PostgreEnums.SkillLevel SkillLevel { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.SkillLevel.beginner;

        public StrateZone_Repository.Parameters.PostgreEnums.Ranking Ranking { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.Ranking.basic;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        /***************************************************ONLY RESPONSE TO AUTH APIS*********************************************************/
        // Refresh Token Fields
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        public string? OTP { get; set; } // Store OTP
        public DateTime? OTPExpiry { get; set; } // OTP expiration time
        /***************************************************************************************************************************************/

        public virtual ICollection<FriendlistModel> FriendlistUsers { get; set; } = new List<FriendlistModel>();

        public virtual ICollection<WalletModel> Wallets { get; set; } = new List<WalletModel>();
    }
}
