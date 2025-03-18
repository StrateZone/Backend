using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IOtpCacheService
    {
        Task StoreOtpAsync(string email, string otp, DateTime expiry);
        Task<(string OTP, DateTime Expiry)?> GetOtpAsync(string email);
    }
}
