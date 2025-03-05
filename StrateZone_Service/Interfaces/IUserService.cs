using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;

namespace StrateZone_Service.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserRequest userRequest);
        Task<UserResponse> DeleteUserAsync(int id);
        Task<UserResponse> GetUserByEmailAsync(string email);
        Task<UserResponse> GetUserByIdAsync(int id);
        Task<List<UserResponse>> GetUsersByUsernameAsync(string username);
        Task<List<UserResponse>> GetUsersAsync();
        Task<UserResponse> UpdateUserAsync(UserModel userModel, int id);
    }
}