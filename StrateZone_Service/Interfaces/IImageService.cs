using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IImageService
    {
        Task<ImageModel> CreateImageAsync(ImageRequest image);
        Task<ImageModel> DeleteImageAsync(int id);
        Task<ImageModel> GetEventThumbnailAsync(int eventId);
        Task<List<ImageModel>> GetProductImagesAsync(int productId);
        Task<List<ImageModel>> GetThreadImagesAsync(int threadId);
        Task<ImageModel> GetGametypeThumbnail(int gametypeId);
        Task<ImageModel> GetTournamentThumbnailAsync(int tournamentId);
        Task<ImageModel> GetUserAvatarAsync(int userId);
        Task<ImageModel> UpdateImageAsync(ImageModel image, int id);
    }
}
