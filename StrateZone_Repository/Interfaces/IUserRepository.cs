using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<int> DeleteUnactivatedAccountsAsync(int daysAfterAccountCreate);
        Task<User> DeleteUserAsync(int id);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<User> GetUserByUsernameAsync(string username);
        Task<PagedList<User>> SearchForFriendsByUsernameAsync(UserListParameters parameters, int id, string? username);
        Task<PagedList<User>> GetUsersAsync(UserListParameters parameters);
        Task<PagedList<User>> GetUsersByRanking(UserListParameters parameters, PostgreEnums.Ranking ranking, int up, int down);
        Task<(List<User>, List<User>)> GetRandomOpponentsAsync(int userId, string SearchTerm);
        Task<PagedList<User>> GetUsersByUsernameAsync(UserListParameters parameters, string username);
        Task<User> UpdateUserAsync(User updatedUser, int id);
        Task<User> FindUserAcceptedToJoinTablesAppointment(TablesAppointment tablesAppointment);
    }
}