using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IGameTypeRepository
    {
        Task<GameType> AddAsync(GameType gameType);
        Task<GameType> UpdateAsync(GameType type, int id);
        Task<GameType> DeleteAsync(int id);
        Task<List<GameType>> GetGameTypesAsync();
        Task<GameType> GetGameTypesByIdAsync(int id);
        Task<List<GameType>> GetActiveGameTypesAsync();
        Task<GameType> GetGameTypeWithExtensionsByIdAsync(int id);
    }
}