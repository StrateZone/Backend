using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> Login(LoginRequest loginRequest);
        Task<ApiResponse<LoginResponse>> RefreshToken(string refreshToken);
    }
}
