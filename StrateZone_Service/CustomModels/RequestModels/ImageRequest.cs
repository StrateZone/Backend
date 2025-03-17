using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public enum ImageType { avatar, game_type, product, thread, event_thumbnail, tournament_thumbnail };

    public class ImageRequest
    {
        public ImageType Type { get; set; }

        public int? EntityId { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }

        public int? Width { get; set; } = null;

        public int? Height { get; set; } = null;
    }
}
