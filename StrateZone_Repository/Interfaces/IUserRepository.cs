using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User> DeleteUserAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<PagedList<User>> GetUsersByUsernameAsync(UserListParameters parameters, string username);
        Task<PagedList<User>> GetUsersAsync(UserListParameters parameters);
        Task<PagedList<User>> GetUsersByRanking(UserListParameters parameters, PostgreEnums.Ranking ranking, int up, int down);
        Task<User> UpdateUserAsync(User user, int id);
    }
}