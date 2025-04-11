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

        public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

        public virtual ICollection<ThreadsTagModel> ThreadsTags { get; set; } = new List<ThreadsTagModel>();
    }
}
