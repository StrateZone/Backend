using AutoMapper;
using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentrequestService _appointmentrequestService;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IWalletService walletService, IMapper mapper, IAppointmentrequestService appointmentrequestService)
        {
            _userRepository = userRepository;
            _walletService = walletService;
            _mapper = mapper;
            _appointmentrequestService = appointmentrequestService;
        }

        public async Task<PagedList<UserResponse>> GetUsersAsync(UserListParameters parameters)
        {
            try
            {
                var results = await _userRepository.GetUsersAsync(parameters);
                var users = _mapper.Map<PagedList<UserResponse>>(results);
                return new PagedList<UserResponse>(users, results.Count, results.CurrentPage, results.PageSize);
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
                return new PagedList<UserResponse>(users, results.Count, results.CurrentPage, results.PageSize);
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
                return new PagedList<UserResponse>(users, results.Count, results.CurrentPage, results.PageSize);
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

        public async Task<SearchedOpponentsResponse> GetRandomUsersByRankingAsync(HashSet<int> excludedIds, int tableId, DateTime StartTime, DateTime EndTime, PostgreEnums.Ranking ranking, int up, int down)
        {
            try
            {
                var requestedUserId = excludedIds.ElementAt(0);

                var results = await _userRepository.GetRandomUsersByRanking(excludedIds, ranking, up, down);
                var users = _mapper.Map<List<UserResponse>>(results);
                var opponents = _mapper.Map<List<OpponentResponse>>(users);

                var appointmentRequestsToUsers = (await _appointmentrequestService.GetCurrentAppointmentRequestsFromUserByUserAndTableIdAsync(requestedUserId, tableId, StartTime, EndTime))
                                            .Select(ar => ar.ToUser).ToArray();

                foreach (var opponent in opponents)
                {
                    opponent.IsInvited = appointmentRequestsToUsers.Contains(opponent.UserId);
                };

                return new SearchedOpponentsResponse()
                {
                    ExcludedIds = [.. excludedIds, .. opponents.Select(o => o.UserId).ToArray()],
                    MatchingOpponents = opponents,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
