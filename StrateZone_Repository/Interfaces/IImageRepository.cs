using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IImageRepository
    {
        Task<Image> CreateImageAsync(Image image);
        Task<Image> DeleteImageAsync(int id);
        Task<Image> GetEventThumbnailAsync(int eventId);
        Task<List<Image>> GetProductImagesAsync(int productId);
        Task<List<Image>> GetThreadImagesAsync(int threadId);
        Task<Image> GetTournamentThumbnailAsync(int tournamentId);
        Task<Image> GetUserAvatarAsync(int userId);
        Task<Image> GetGametypeThumbnailAsync(int gametypeId);
        Task<Image> UpdateImageAsync(Image image, int id);
    }
}