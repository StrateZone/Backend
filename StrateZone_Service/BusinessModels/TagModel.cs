using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class TagModel
    {
        public int TagId { get; set; }

        public string? TagName { get; set; }

        public string? Status { get; set;}

        public string AllowedRole { get; set; }

        public string TagColor { get; set; }
    }
}
