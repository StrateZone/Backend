using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
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

        private static readonly string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string lowercase = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string digits = "0123456789";
        private static readonly string specialChars = "!@#$%^&*.+=-";
        private static readonly string allChars = lowercase + uppercase + digits + specialChars;
        private static readonly Random random = new Random();

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IEmailService emailService, IMapper mapper, IWalletRepository walletRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _mapper = mapper;
            _walletRepository = walletRepository;
        }

        public static string GenerateSecureString(int length)
        {
            if (length < 8) length = 8;

            var result = new StringBuilder();

            result.Append(uppercase[random.Next(uppercase.Length)]);

            char numberChar = digits[random.Next(digits.Length)];
            char specialChar = specialChars[random.Next(specialChars.Length)];

            for (int i = 0; i < length - 3; i++)
            {
                result.Append(allChars[random.Next(allChars.Length)]);
            }

            // Add guaranteed number and special char
            result.Append(numberChar);
            result.Append(specialChar);

            char[] finalChars = result.ToString().ToCharArray();
            Shuffle(finalChars, 1);

            return new string(finalChars);
        }

        private static void Shuffle(char[] array, int startIndex)
        {
            for (int i = array.Length - 1; i > startIndex; i--)
            {
                int j = random.Next(startIndex, i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
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
                user.OTPExpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified).AddSeconds(5 * 60); // OTP valid for 5 minutes

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified).AddDays(7);

                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

                EmailRequest emailSending;

                if (user.Status == PostgreEnums.UserStatus.Unactivated)
                {
                    emailSending = new EmailRequest
                    {
                        ToEmail = email,
                        Subject = "Account Verification",
                        Content = $"<p>Mã xác thực của bạn là:</p><h1><b>{updatedUser.OTP}</b></h1><p>Mã này có hiệu lực trong vòng 5 phút.<br>Vui lòng không chia sẻ mã này cho bất kì ai. Nếu mã này không phải do bạn yêu cầu, vui lòng bỏ qua.</p>"
                    };
                }
                else
                {
                    emailSending = new EmailRequest
                    {
                        ToEmail = email,
                        Subject = "OTP Verification",
                        Content = $"<p>Mã xác thực của bạn là:</p><h1><b>{updatedUser.OTP}</b></h1><p>Mã này có hiệu lực trong vòng 5 phút.<br>Vui lòng không chia sẻ mã này cho bất kì ai. Nếu mã này không phải do bạn yêu cầu, vui lòng bỏ qua.</p>"
                    };
                }

                var mailMessage = await _emailService.SendEmailAsync(emailSending);

                return new ApiResponse<MailMessage> 
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "OTP sent",
                    Data = null,
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

            if (user.RefreshTokenExpiry < DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified))
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
            user.RefreshTokenExpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified).AddDays(7);
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
            return random.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString();
        }

        public async Task<ApiResponse<MailMessage>> SendNewPassword(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse<MailMessage> { Success = false, StatusCode = 404, Message = "User doesnt exist", Data = null };
                }

                string newPassword = GenerateSecureString(24);

                user.Password = new PasswordHasher<string>().HashPassword(null, newPassword);
                user.IsPasswordHashed = true;
                var updatedUser = await _userRepository.UpdateUserAsync(user, user.UserId);

                EmailRequest emailSending= new EmailRequest
                {
                    ToEmail = email,
                    Subject = "Khôi phục mật khẩu",
                    Content = $"<p>Mật khẩu mới của bạn là:</p><h1 style=\"background-color:yellow\"><b>{newPassword}</b></h1><p>Sau khi đăng nhập thành công, vui lòng vào hồ sơ để đổi mật khẩu.</p>"
                };
                
                await _emailService.SendEmailAsync(emailSending);

                return new ApiResponse<MailMessage>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "OTP sent",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

                if (user.OTP != loginRequest.OTP || user.OTPExpiry < DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified))
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 401, Message = "Invalid or expired OTP", Data = null };

                if (user.Status == PostgreEnums.UserStatus.Suspended)
                {
                    return new ApiResponse<LoginResponse> 
                    { 
                        Success = false, 
                        StatusCode = 401, 
                        Message = "Tài khoản này hiện đang bị cấm do vi phạm tiêu chuẩn cộng đồng. " +
                        "Mọi thắc mắc vui lòng liên hệ: stratezone.app@gmail.com", 
                        Data = null 
                    };
                }

                user.OTP = null;
                user.OTPExpiry = null;

                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified).AddDays(7);

                if (user.Status == PostgreEnums.UserStatus.Unactivated) user.Status = PostgreEnums.UserStatus.Active;

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
                        Status = updatedUser.Status.ToString(),
                        Gender = updatedUser.Gender.ToString(),
                        FullName = updatedUser.FullName,
                        Address = updatedUser.Address,
                        Bio = updatedUser.Bio,
                        ImageUrl = updatedUser.AvatarUrl,
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

        public async Task<ApiResponse<LoginResponse>> LoginPassword(PasswordLoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 404, Message = "User doesnt exist", Data = null };
                }

                var verification = new PasswordHasher<string>().VerifyHashedPassword(null, user.Password, loginRequest.Password);

                if (verification == PasswordVerificationResult.Failed)
                    return new ApiResponse<LoginResponse> { Success = false, StatusCode = 401, Message = "Invalid email or password", Data = null };

                if (user.Status == PostgreEnums.UserStatus.Unactivated)
                    return new ApiResponse<LoginResponse> 
                    { 
                        Success = false, 
                        StatusCode = 404, 
                        Message = "Tài khoản chưa được xác thực email.", 
                        Data = null 
                    };

                if (user.Status == PostgreEnums.UserStatus.Suspended)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        StatusCode = 401,
                        Message = "Tài khoản này hiện đang bị cấm do vi phạm tiêu chuẩn cộng đồng. " +
                        "Mọi thắc mắc vui lòng liên hệ: stratezone.app@gmail.com",
                        Data = null
                    };
                }

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
                        Status = updatedUser.Status.ToString(),
                        Gender = updatedUser.Gender.ToString(),
                        FullName = updatedUser.FullName,
                        Address = updatedUser.Address,
                        Bio = updatedUser.Bio,
                        ImageUrl = updatedUser.AvatarUrl,
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
                    Password = new PasswordHasher<string>().HashPassword(null, registerRequest.Password),
                    UserLabel = PostgreEnums.UserLabel.none.ToString(),
                    Gender = registerRequest.Gender,
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
