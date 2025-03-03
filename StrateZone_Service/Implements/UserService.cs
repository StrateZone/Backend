using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            try
            {
                var results = await _userRepository.GetUsersAsync();
                return _mapper.Map<List<UserModel>>(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            try
            {
                var results = await _userRepository.GetUserByIdAsync(id);
                return _mapper.Map<UserModel>(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            try
            {
                var results = await _userRepository.GetUserByEmailAsync(email);
                return _mapper.Map<UserModel>(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            try
            {
                var results = await _userRepository.GetUserByUsernameAsync(username);
                return _mapper.Map<UserModel>(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserModel> CreateUserAsync(UserRequest userRequest)
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
                    Gender = (StrateZone_Repository.Parameters.PostgreEnums.Gender) userRequest.Gender,
                    CreatedAt = DateTime.UtcNow,
                };

                var user = _mapper.Map<User>(userModel);
                var result = await _userRepository.CreateUserAsync(user);

                return _mapper.Map<UserModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserModel> UpdateUserAsync(UserModel userModel, int id)
        {
            try
            {
                var user = _mapper.Map<User>(userModel);
                var result = await _userRepository.UpdateUserAsync(user, id);

                return _mapper.Map<UserModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserModel> DeleteUserAsync(int id)
        {
            try
            {
                var result = await _userRepository.DeleteUserAsync(id);

                return _mapper.Map<UserModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
