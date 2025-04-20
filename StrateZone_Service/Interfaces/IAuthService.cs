using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<UserResponse>> RegisterAccount(RegisterRequest registerRequest);
        Task<ApiResponse<RefreshTokenResponse>> RefreshToken(string refreshToken);
        Task<ApiResponse<MailMessage>> SendOTP(string email);
        Task<ApiResponse<LoginResponse>> VerifyOTP(EmailLoginRequest loginRequest);
        Task<ApiResponse<LoginResponse>> LoginPassword(PasswordLoginRequest loginRequest);
    }
}
