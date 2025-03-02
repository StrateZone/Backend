using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface IGameTypeService
    {
        Task<GameTypeModel> GetGameTypeByIdAsync(int id);
        Task<List<GameTypeModel>> GetGameTypesAsync();
        Task<List<GameTypeModel>> GetGameTypesWithExtensionsAsync();
        Task<GameTypeModel> GetGameTypeWithExtensionsByIdAsync(int id);

        Task<GameTypeModel> GetGameTypeByGameExtensionIdAsync(int id);
    }
}