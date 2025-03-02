using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> CreateUserAsync(UserRequest userRequest);
        Task<UserModel> DeleteUserAsync(int id);
        Task<UserModel> GetUserByEmailAsync(string email);
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel> UpdateUserAsync(UserModel userModel, int id);
    }
}