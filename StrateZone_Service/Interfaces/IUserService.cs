using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserRequest userRequest);
        Task<UserResponse> DeleteUserAsync(int id);
        Task<UserResponse> GetUserByEmailAsync(string email);
        Task<UserResponse> GetUserByIdAsync(int id);
        Task<UserResponse> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<PagedList<UserResponse>> GetUsersByUsernameAsync(UserListParameters parameters, string username);
        Task<PagedList<UserResponse>> GetUsersAsync(UserListParameters parameters);
        Task<PagedList<UserResponse>> GetUsersByRankingAsync(UserListParameters parameters, Ranking ranking, int up, int down);
        Task<UserResponse> UpdateUserAsync(UserModel userModel, int id);
        Task<int> DeleteUnactivatedAccountsAsync(int daysAfterAccountCreate);
        Task<UserResponse> FindUserInvitedToTablesAppointment(TablesAppointmentModel tablesAppointment);
    }
}