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
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IImageRepository imageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<PagedList<UserResponse>> GetUsersAsync(UserListParameters parameters)
        {
            try
            {
                var results = await _userRepository.GetUsersAsync(parameters);
                var users = _mapper.Map<PagedList<UserResponse>>(results);
                return new PagedList<UserResponse>(users, users.Count, users.CurrentPage, users.PageSize);
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
                return new PagedList<UserResponse>(users, users.Count, users.CurrentPage, users.PageSize);
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
                return new PagedList<UserResponse>(users, users.Count, users.CurrentPage, users.PageSize);
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
                    CreatedAt = DateTime.UtcNow,
                };

                var user = _mapper.Map<User>(userModel);
                var result = await _userRepository.CreateUserAsync(user);

                return _mapper.Map<UserResponse>(result);
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
    }
}
