using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class UserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [Length(6, 24)]
        public string UserName { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Required]
        public Gender? Gender { get; set; } = StrateZone_Repository.Parameters.PostgreEnums.Gender.male;
    }
}
