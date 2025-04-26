using Microsoft.AspNetCore.Http;
using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class ThreadRequest
    {
        public int? CreatedBy { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public virtual HashSet<int> TagIds { get; set; } = new();

        public bool isDrafted { get; set; } = false;
    }

    public class ThreadEditRequest
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public virtual HashSet<int> TagIds { get; set; } = new();
    }
}
