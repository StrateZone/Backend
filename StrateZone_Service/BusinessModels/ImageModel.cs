using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class ImageModel
    {
        public int ImageId { get; set; }

        public int? UserId { get; set; }

        public int? ProductId { get; set; }

        public int? ThreadId { get; set; }

        public int? GameTypeId { get; set; }

        public int? TournamentId { get; set; }

        public int? EventId { get; set; }

        public string? Url { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
