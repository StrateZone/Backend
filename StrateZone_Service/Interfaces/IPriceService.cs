using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Interfaces
{
    public interface IPriceService
    {
        Task<PriceModel> GetMembershipPriceAsync();
        Task<PriceModel> GetPriceOfGameTypeAsync(PostgreEnums.GameTypeEnum gameType);
        Task<PriceModel> GetPriceOfRoomTypeAsync(PostgreEnums.RoomType roomType);
        Task<PriceModel> GetProductPriceByIdAsync(int productId);
        Task<PagedList<PriceModel>> GetServicePricesAsync(PriceParameters parameters);
        Task<PriceModel> GetTeachingSalaryAsync();
        Task<PriceModel> UpdatePriceAsync(PriceModel priceModel, int id);
    }
}