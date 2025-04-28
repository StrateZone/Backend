using AutoMapper;
using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System.Globalization;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentrequestService _appointmentrequestService;
        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IWalletService walletService, IMapper mapper, IAppointmentrequestService appointmentrequestService, IPaymentService paymentService)
        {
            _userRepository = userRepository;
            _walletService = walletService;
            _mapper = mapper;
            _appointmentrequestService = appointmentrequestService;
            _paymentService = paymentService;
        }

        public async Task<PagedList<UserResponse>> GetUsersAsync(UserListParameters parameters)
        {
            try
            {
                var results = await _userRepository.GetUsersAsync(parameters);
                var users = _mapper.Map<PagedList<UserResponse>>(results);
                return new PagedList<UserResponse>(users, results.TotalCount, results.CurrentPage, results.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<UserDashboardResponse>> GetUsersDashboardAsync()
        {
            try
            {
                var results = await _userRepository.GetUsersDashboardAsync();
                var users = _mapper.Map<List<UserDashboardResponse>>(results);
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<UserResponse>> GetUsersByRankingAsync(UserListParameters parameters, PostgreEnums.Ranking ranking, int up, int down)
        {
            try
            {
                var results = await _userRepository.GetUsersByRanking(parameters, ranking, up, down);
                var users = _mapper.Map<PagedList<UserResponse>>(results);
                return new PagedList<UserResponse>(users, results.TotalCount, results.CurrentPage, results.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            try
            {
                var results = await _userRepository.GetUserByIdAsync(id);
                return _mapper.Map<UserResponse>(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> GetUserByEmailAsync(string email)
        {
            try
            {
                var results = await _userRepository.GetUserByEmailAsync(email);
                return _mapper.Map<UserResponse>(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                var results = await _userRepository.GetUserByPhoneNumberAsync(phoneNumber);
                return _mapper.Map<UserResponse>(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<UserResponse>> GetUsersByUsernameAsync(UserListParameters parameters, string username)
        {
            try
            {
                var results = await _userRepository.GetUsersByUsernameAsync(parameters, username);
                var users = _mapper.Map<PagedList<UserResponse>>(results);
                return new PagedList<UserResponse>(users, results.TotalCount, results.CurrentPage, results.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<PagedList<FriendResponse>> SearchForFriendsByUsernameAsync(UserListParameters parameters, int id, string? username)
        {
            try
            {
                var results = await _userRepository.SearchForFriendsByUsernameAsync(parameters, id, username);
                
                var usersResponse = _mapper.Map<PagedList<UserResponse>>(results.Item1);
                
                var friends = _mapper.Map<PagedList<FriendResponse>>(usersResponse);
                foreach (var f in friends)
                {
                    if (results.Item2.Contains(f.UserId)) f.FriendStatus = FriendStatus.friended;
                    else if (results.Item3.Contains(f.UserId)) f.FriendStatus = FriendStatus.request_sent;
                    else f.FriendStatus = FriendStatus.stranger;
                }

                return new PagedList<FriendResponse>(
                            friends, 
                            results.Item1.TotalCount, 
                            results.Item1.CurrentPage, 
                            results.Item1.PageSize
                        );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> CreateUserAsync(UserRequest userRequest)
        {
            try
            {
                UserModel userModel = new UserModel()
                {
                    Username = userRequest.UserName,
                    Password = userRequest.Password,
                    Email = userRequest.Email,
                    Phone = userRequest.PhoneNumber,
                    Address = userRequest.Address,
                    Gender = (StrateZone_Repository.Parameters.PostgreEnums.Gender)userRequest.Gender,
                    SkillLevel = (StrateZone_Repository.Parameters.PostgreEnums.SkillLevel)userRequest.SkillLevel,
                    CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    Status = "Unactivated"
                };

                var user = _mapper.Map<User>(userModel);
                var result = await _userRepository.CreateUserAsync(user);
                var userResponse = _mapper.Map<UserResponse>(result);

                WalletModel walletModel = new WalletModel()
                {
                    UserId = result.UserId,
                    Balance = 0,
                    Status = PostgreEnums.WalletStatus.active,
                };

                var wallet = await _walletService.CreateWalletAsync(walletModel);
                userResponse.Wallet = wallet;

                return userResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> UpdateUserAsync(UserModel userModel, int id)
        {
            try
            {
                var user = _mapper.Map<User>(userModel);
                var result = await _userRepository.UpdateUserAsync(user, id);

                return _mapper.Map<UserResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> SuspendUserAccount(int id)
        {
            try
            {
                var find = await GetUserByIdAsync(id);

                if (find.Status == UserStatus.Suspended.ToString()) throw new Exception("This account is already suspended");

                var result = await _userRepository.UpdateUserAsync(new() { Status = UserStatus.Suspended }, id);

                return _mapper.Map<UserResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> KickUserFromCommunityAsync(int id)
        {
            try
            {
                var find = await GetUserByIdAsync(id);

                if (find.UserRole == UserRole.RegisteredUser.ToString()) throw new Exception("This account is not a member of community");

                var result = await _userRepository.UpdateUserAsync(new() { UserRole = UserRole.RegisteredUser }, id);

                return _mapper.Map<UserResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserResponse> DeleteUserAsync(int id)
        {
            try
            {
                var result = await _userRepository.DeleteUserAsync(id);

                return _mapper.Map<UserResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> DeleteUnactivatedAccountsAsync(int daysAfterAccountCreate)
        {
            try
            {
                var accountsDeleted = await _userRepository.DeleteUnactivatedAccountsAsync(daysAfterAccountCreate);

                return accountsDeleted;
            }
            catch
            {
                throw;
            }
        }

        public async Task<UserResponse> FindUserAcceptedToJoinTablesAppointment(TablesAppointmentModel tablesAppointmentModel)
        {
            try
            {
                var tablesAppointment = _mapper.Map<TablesAppointment>(tablesAppointmentModel);
                var user = await _userRepository.FindUserAcceptedToJoinTablesAppointment(tablesAppointment);

                return user == null ? null : _mapper.Map<UserResponse>(user);
            }
            catch
            {
                throw;
            }
        }

        public async Task<SearchedOpponentsResponse> GetRandomOpponentsAsync(int userId, string? SearchTerm, HashSet<int> excludedIds)
        {
            try
            { 
                var results = await _userRepository.GetRandomOpponentsAsync(userId, SearchTerm, excludedIds);

                var mappedResults1 = _mapper.Map<List<UserResponse>>(results.Item1);
                var mappedResults2 = _mapper.Map<List<UserResponse>>(results.Item2);

                return new SearchedOpponentsResponse()
                {
                    ExcludedIds = results.Item3,
                    MatchingOpponents = _mapper.Map<List<OpponentResponse>>(mappedResults1),
                    Friends = _mapper.Map<List<OpponentResponse>>(mappedResults2),
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> CheckUserNotification(int id)
        {
            try
            {
                PaymentParameters paymentParams = new PaymentParameters()
                {
                    PageNumber = 1,
                    PaymentStatuses = [ PaymentStatus.unpaid ],
                    PaymentTypes = [ PaymentType.appointment ],
                    PageSize = 100_000
                };

                var pendingPayments = await _paymentService.GetPaymentsByUserIdAsync(id, paymentParams);
                if (pendingPayments.Count > 0) return true;

                AppointmentRequestParameters requestParameters = new AppointmentRequestParameters()
                { 
                    PageNumber = 1,
                    PageSize = 100_000
                };
                
                var requests = await _appointmentrequestService.GetAppointmentRequestsOfUserByUserIdAsync(requestParameters, id);

                return requests.Any(r => r.Status == "pending");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task AssignTopContributorsAsync()
        {
            try
            { 
                await _userRepository.AssignTopContributorsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task UpdateExpiredMemberships()
        {
            try
            {
                await _userRepository.UpdateExpiredMemberships();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserMonthResponse> GetUsersJoinedInAMonth(int month, int year)
        {
            try
            {
                List<UserDailyResponse> userDailyResponses = new();

                int dayInMonth = DateTime.DaysInMonth(year, month);
                for (int i = 1; i <= dayInMonth; ++i)
                {
                    int userJoined = (await _userRepository.GetNewUserWithinDayAsync(i, month, year)).Count();

                    userDailyResponses.Add(new() { DayOfMonth = i, UsersJoined = userJoined });
                }

                return new()
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalDays = dayInMonth,
                    UserDailyResponses = userDailyResponses
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
