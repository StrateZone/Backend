using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IGameTypeService
    {
        Task<GameTypeModel> AddAsync(GameTypeRequest request);
        Task<GameTypeModel> DeleteAsync(int id);
        Task<GameTypeModel> GetGameTypeByIdAsync(int id);
        Task<List<GameTypeModel>> GetGameTypesAsync();
        Task<List<GameTypeModel>> GetGameTypesWithExtensionsAsync();
        Task<GameTypeModel> GetGameTypeWithExtensionsByIdAsync(int id);
    }
}