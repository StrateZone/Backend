using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface IPriceRepository
    {
        Task<Price> GetMembershipPriceAsync();
        Task<Price> GetPriceOfGameTypeAsync(GameTypeEnum gameType);
        Task<Price> GetPriceOfRoomTypeAsync(RoomType roomType);
        Task<Price> GetProductPriceByIdAsync(int productId);
        Task<PagedList<Price>> GetServicePrices(PriceParameters parameters);
        Task<Price> GetTeachingSalaryAsync();
        Task<Price> UpdatePriceAsync(Price price, int id);
    }
}