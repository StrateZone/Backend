using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IGameTypeRepository
    {
        Task<GameType> AddAsync(GameType gameType);
        Task<GameType> DeleteAsync(int id);
        Task<List<GameType>> GetGameTypesAsync();
        Task<GameType> GetGameTypesByIdAsync(int id);
        Task<List<GameType>> GetGameTypesWithExtensionsAsync();
        Task<GameType> GetGameTypeWithExtensionsByIdAsync(int id);
    }
}