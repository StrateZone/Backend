using Newtonsoft.Json.Linq;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
                if (user == null || user.Password != loginRequest.Password)
                {
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 401, Message="Invalid email or password", Data = null };
                }

                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);



                return new ApiResponse<LoginResponse> 
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Login success",
                    Data = new LoginResponse
                    {
                        UserId = updatedUser.UserId,
                        Username = updatedUser.Username,
                        Email = updatedUser.Email,
                        Phone = updatedUser.Phone,
                        AccessToken = newAccessToken,
                        RefreshToken = updatedUser.RefreshToken
                    }
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApiResponse<RefreshTokenResponse>> RefreshToken(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
                return null;
            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                return new ApiResponse<RefreshTokenResponse>
                {
                    Success = false,
                    Message = "Expired refresh token",
                    StatusCode = 401,
                    Data = null
                };

            var newToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

            return new ApiResponse<RefreshTokenResponse>
            {
                Success = true,
                Message = "New token generated",
                StatusCode = 200,
                Data = new RefreshTokenResponse
                {
                    NewToken = newToken,
                    RefreshToken = updatedUser.RefreshToken
                }
            };
        }
    }
}
