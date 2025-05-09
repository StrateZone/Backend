using StrateZone_Repository.Pagination;
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
        Task<PagedList<FriendResponse>> SearchForFriendsByUsernameAsync(UserListParameters parameters, int id, string? username);
        Task<PagedList<UserResponse>> GetUsersAsync(UserListParameters parameters);
        Task<PagedList<UserManagementResponse>> GetUsersManagementAsync(UserListManagementParameters parameters);
        Task<List<UserDashboardResponse>> GetUsersDashboardAsync();
        Task<PagedList<UserResponse>> GetUsersByRankingAsync(UserListParameters parameters, Ranking ranking, int up, int down);
        Task<SearchedOpponentsResponse> GetRandomOpponentsAsync(int userId, string? SearchTerm, HashSet<int> excludedIds);
        Task<UserResponse> UpdateUserAsync(UserModel userModel, int id);
        Task<UserResponse> EditUserProfileAsync(UserModel userModel, int id);
        Task<UserResponse> SuspendUserAccount(int id);
        Task<List<UserResponse>> PasswordUserAsync();
        Task<UserResponse> KickUserFromCommunityAsync(int id);
        Task<int> DeleteUnactivatedAccountsAsync(int daysAfterAccountCreate);
        Task<UserResponse> FindUserAcceptedToJoinTablesAppointment(TablesAppointmentModel tablesAppointment);
        Task<bool> CheckUserNotification(int id);
        Task<int> GetUsersJoinedInAMonth(int month, int year);
        Task<UserResponse> ChangePasswordAsync(int userId, string oldPassword, string newPassword, string confirmPassword);
        Task<UserResponse> ForgotPasswordAsync(int userId, string newPassword, string confirmPassword);
        Task<UserModel> GetUserByAppointmentIdAsync(int id);
        Task AssignTopContributorsAsync();
        Task UpdateExpiredMemberships();
        Task<(int, int)> GetUserPointsAsync(int id);
        Task<string> GetUserRole(int id);
    }
}