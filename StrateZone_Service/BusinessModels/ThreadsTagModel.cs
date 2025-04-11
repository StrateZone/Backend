using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class ThreadsTagModel
    {
        public int Id { get; set; }

        public int? ThreadId { get; set; }

        public int? TagId { get; set; }

        public virtual TagModel? Tag { get; set; }
    }
}
