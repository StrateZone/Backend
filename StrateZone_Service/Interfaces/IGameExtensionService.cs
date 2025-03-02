using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface IGameExtensionService
    {
        Task<List<GameExtensionModel>> GetGameExtensionsAsync();
        Task<List<GameExtensionModel>> GetGameExtensionsByGameTypeIdAsync(int id);
        Task<GameExtensionModel> GetGameExtensionByIdAsync(int id);
    }
}