using Microsoft.AspNetCore.Http;
using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public enum ImageType { avatar, game_type, product, thread, event_thumbnail, tournament_thumbnail };

    public class ImageRequest
    {
        public ImageType Type { get; set; }

        public int? Width { get; set; } = null;

        public int? Height { get; set; } = null;

        public int? EntityId { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }
}
