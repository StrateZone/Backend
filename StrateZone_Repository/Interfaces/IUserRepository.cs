using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User> DeleteUserAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetUsersByUsernameAsync(string username);
        Task<List<User>> GetUsersAsync();
        Task<User> UpdateUserAsync(User user, int id);
    }
}