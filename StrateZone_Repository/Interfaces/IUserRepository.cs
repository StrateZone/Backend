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
        Task<List<User>> GetUsersByUsernameAsync(string username);
        Task<List<User>> GetUsersAsync();
        // Task<List<User>> GetUsersBySkillLevel(PostgreEnums.SkillLevel skillLevel, int margin);
        Task<User> UpdateUserAsync(User user, int id);
    }
}