using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface ILikeRepository
    {
        Task<Like> CreateLike(Like like);
        Task<Like> DeleteLike(int id);
    }
}