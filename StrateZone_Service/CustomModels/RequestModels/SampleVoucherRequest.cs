using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class SampleVoucherRequest
    {
        [Required]
        public string VoucherName { get; set; }

        [Required]
        public int Value { get; set; }

        [Required]
        public int PointsCost {  get; set; }

        [Required]
        public int ContributorPointsCost { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal MinPriceCondition { get; set; }
    }

    public class UserVoucherRequest
    {
        [Required]
        public int SampleVoucherId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
