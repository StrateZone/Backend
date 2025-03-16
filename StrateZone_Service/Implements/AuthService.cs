using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
        }
        public async Task<ApiResponse<MailMessage>> SendOTP(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse<MailMessage> { Success = false, StatusCode = 404, Message="User doesnt exist", Data = null };
                }

                //var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                string otp = GenerateOTP();

                user.OTP = otp;
                user.OTPExpiry = DateTime.UtcNow.AddSeconds(40); // OTP valid for 5 minutes

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

                var emailSending = new EmailRequest
                {
                    ToEmail = email,
                    Subject = "Login Verification",
                    Content = "Your login OTP is " + updatedUser.OTP
                };

                var mailMessage = await _emailService.SendEmailAsync(emailSending);

                return new ApiResponse<MailMessage> 
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "OTP sent",
                    Data = mailMessage
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

        private string GenerateOTP(int length = 6)
        {
            Random random = new Random();
            return random.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString();
        }

        public async Task<ApiResponse<LoginResponse>> VerifyOTP(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 404, Message = "User doesnt exist", Data = null };
                }

                if (user.OTP != loginRequest.OTP || user.OTPExpiry < DateTime.UtcNow)
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 401, Message = "Invalid or expired OTP", Data = null };

                user.OTP = null;
                user.OTPExpiry = null;

                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "OTP sent",
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
    }
}
