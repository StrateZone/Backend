using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class TransactionModel
    {
        public int Id { get; set; }

        public int? OfUser { get; set; }

        public string? ReferenceId { get; set; }

        public string? Content { get; set; }

        public decimal? Amount { get; set; }

        public TransactionType TransactionType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual UserModel? OfUserNavigation { get; set; }
    }
}
