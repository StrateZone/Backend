using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IFriendrequestRepository
    {
        Task<PagedList<Friendrequest>> GetFriendrequestsOfUserIdAsync(FriendrequestParameters parameters, int id);
        Task<Friendrequest> GetFriendrequestByIdAsync(int id);
        Task<Friendrequest> CreateFriendrequestAsync(Friendrequest friendrequest);
        Task<Friendrequest> UpdateFriendrequestAsync(Friendrequest friendrequest, int id);
        Task<Friendrequest> DeleteFriendrequestAsync(int id);
    }
}
