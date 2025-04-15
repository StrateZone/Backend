using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
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
        private readonly IMapper _mapper;
        private readonly IWalletRepository _walletRepository;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IEmailService emailService, IMapper mapper, IWalletRepository walletRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _mapper = mapper;
            _walletRepository = walletRepository;
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
                user.OTPExpiry = DateTime.UtcNow.AddSeconds(5 * 60); // OTP valid for 5 minutes

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

                EmailRequest emailSending;

                if (user.Status == "Unactivated")
                {
                    emailSending = new EmailRequest
                    {
                        ToEmail = email,
                        Subject = "Account Verification",
                        Content = $"<p>Mã OTP kích hoạt của bạn là:</p><h1><b>{updatedUser.OTP}</b></h1><p>OTP này có hiệu lực trong vòng 5 phút.<br>Vui lòng không chia sẻ mã này cho bất kì ai. Nếu mã này không phải do bạn yêu cầu, vui lòng bỏ qua.</p>"
                    };
                }
                else
                {
                    emailSending = new EmailRequest
                    {
                        ToEmail = email,
                        Subject = "Login Verification",
                        Content = $"<p>Mã OTP đăng nhập của bạn là:</p><h1><b>{updatedUser.OTP}</b></h1><p>OTP này có hiệu lực trong vòng 5 phút.<br>Vui lòng không chia sẻ mã này cho bất kì ai. Nếu mã này không phải do bạn yêu cầu, vui lòng bỏ qua.</p>"
                    };
                }

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

        public async Task<ApiResponse<LoginResponse>> VerifyOTP(EmailLoginRequest loginRequest)
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

                if (user.Status == "Unactivated") user.Status = "Active";

                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

                var userWallet = await _walletRepository.GetWalletByUserIdAsync(user.UserId);

                var walletResponse = new WalletResponse {
                    UserId = userWallet.UserId,
                    Balance = userWallet.Balance,
                    Status = userWallet.Status.ToString(),
                };

                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Login successfully!",
                    Data = new LoginResponse
                    {
                        UserId = updatedUser.UserId,
                        UserRole = updatedUser.UserRole.ToString(),
                        Status = updatedUser.Status,
                        Gender = updatedUser.Gender.ToString(),
                        SkillLevel = updatedUser.SkillLevel.ToString(),
                        Ranking = updatedUser.Ranking.ToString(),
                        FullName = updatedUser.FullName,
                        Address = updatedUser.Address,
                        Bio = updatedUser.Bio,
                        ImageUrl = updatedUser.Image?.Url,
                        Wallet = walletResponse,
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

        public async Task<ApiResponse<LoginResponse>> VerifyLogin(PasswordLoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 404, Message = "User doesnt exist", Data = null };
                }

                if (user.Password != loginRequest.Password || user.OTPExpiry < DateTime.UtcNow)
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 401, Message = "Invalid email or password", Data = null };

                user.OTP = null;
                user.OTPExpiry = null;

                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

                var userWallet = await _walletRepository.GetWalletByUserIdAsync(user.UserId);

                var walletResponse = new WalletResponse
                {
                    UserId = userWallet.UserId,
                    Balance = userWallet.Balance,
                    Status = userWallet.Status.ToString(),
                };

                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Login successfully!",
                    Data = new LoginResponse
                    {
                        UserId = updatedUser.UserId,
                        UserRole = updatedUser.UserRole.ToString(),
                        Status = updatedUser.Status,
                        Gender = updatedUser.Gender.ToString(),
                        SkillLevel = updatedUser.SkillLevel.ToString(),
                        Ranking = updatedUser.Ranking.ToString(),
                        FullName = updatedUser.FullName,
                        Address = updatedUser.Address,
                        Bio = updatedUser.Bio,
                        ImageUrl = updatedUser.Image?.Url,
                        Wallet = walletResponse,
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


        public async Task<ApiResponse<UserResponse>> RegisterAccount(RegisterRequest registerRequest)
        {
            try
            {
                UserListParameters userListParameters = new UserListParameters()
                { 
                    PageNumber = 1,
                    PageSize = 100_000
                };

                var existingUsers = await _userRepository.GetUsersAsync(userListParameters);

                foreach (var existingUser in existingUsers)
                {
                    if (existingUser.Email != null && existingUser.Email.Equals(registerRequest.Email))
                        throw new Exception("This email already exists.");
                    else if (existingUser.Username != null && existingUser.Username.Equals(registerRequest.UserName))
                        throw new Exception("This username already exists.");
                    else if (existingUser.Phone != null && existingUser.Phone.Equals(registerRequest.PhoneNumber))
                        throw new Exception("This phone number already exists.");
                }

                UserModel userModel = new()
                {
                    Email = registerRequest.Email,
                    Phone = registerRequest.PhoneNumber,
                    Username = registerRequest.UserName,
                    Address = registerRequest.Address,
                    FullName = registerRequest.FullName,
                    Password = "",
                    Gender = registerRequest.Gender,
                    SkillLevel = StrateZone_Repository.Parameters.PostgreEnums.SkillLevel.beginner,
                    Ranking = StrateZone_Repository.Parameters.PostgreEnums.Ranking.basic,
                    Status = "Unactivated"
                };

                var user = _mapper.Map<User> (userModel);

                var createdUser = await _userRepository.CreateUserAsync(user);

                WalletModel walletModel = new()
                { 
                    UserId = createdUser.UserId,
                    Balance = 0,
                    Status = PostgreEnums.WalletStatus.active,
                };

                await _walletRepository.CreateWalletAsync(_mapper.Map<Wallet>(walletModel));

                await SendOTP(createdUser.Email);

                return new ApiResponse<UserResponse>
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Account created successfully!",
                    Data = _mapper.Map<UserResponse>(createdUser)
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
