using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class OtpCacheService : IOtpCacheService
    {
        private readonly IDistributedCache _cache;

        public OtpCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task StoreOtpAsync(string email, string otp, DateTime expiry)
        {
            var otpData = new { OTP = otp, Expiry = expiry };
            var otpJson = JsonConvert.SerializeObject(otpData);
            await _cache.SetStringAsync(email, otpJson, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expiry
            });
        }

        public async Task<(string OTP, DateTime Expiry)?> GetOtpAsync(string email)
        {
            var otpJson = await _cache.GetStringAsync(email);
            if (otpJson == null) return null;

            var otpData = JsonConvert.DeserializeObject<dynamic>(otpJson);
            return (otpData.OTP.ToString(), (DateTime)otpData.Expiry);
        }
    }
}
