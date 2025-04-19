using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Length(6, 24)]
        public string UserName { get; set; }

        [Phone]
        [Required]
        public string PhoneNumber { get; set; }

        public string? FullName { get; set; }

        public string? Address { get; set; }

        [Required]
        public Gender Gender { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.Gender.male;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Mật khẩu phải dài ít nhất 8 kí tự, trong đó phải bao gồm ít nhất 1 chữ số, 1 kí tự đặc biệt, 1 kí tự in thường và 1 kí tự in hoa.")]
        public string Password { get; set; }
    }
}
