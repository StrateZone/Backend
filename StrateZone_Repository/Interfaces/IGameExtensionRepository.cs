using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IGameExtensionRepository
    {
        Task<GameExtension> GetGameExtensionByIdAsync(int id);
        Task<List<GameExtension>> GetGameExtensionsAsync();
        Task<List<GameExtension>> GetGameExtensionsByGameTypeIdAsync(int id);
    }
}