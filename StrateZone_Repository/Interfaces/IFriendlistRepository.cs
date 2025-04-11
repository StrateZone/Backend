using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IFriendlistRepository
    {
        Task<Friendlist> AddFriendAsync(Friendlist friend);
        Task<Friendlist> DeleteFriendAsync(int id);
        Task<Friendlist> GetFriendByIdAsync(int id);
        Task<PagedList<Friendlist>> GetFriendsByUserIdAsync(TablesAppointmentParameters parameters, int userId);
        Task<Friendlist> UpdateFriendAsync(Friendlist friendlist, int id);
    }
}