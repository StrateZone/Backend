using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class MessageRequest
    {
        public int? SenderId { get; set; }

        public int? ReceiverId { get; set; }

        public string? Content { get; set; }
    }
}
