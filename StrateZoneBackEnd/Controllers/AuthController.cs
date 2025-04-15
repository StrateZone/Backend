using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP(string email)
        {
            try
            {
                var result = await _authService.SendOTP(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount([FromBody] StrateZone_Service.CustomModels.RequestModels.RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                    throw new Exception("Email is required.");
                else if (string.IsNullOrEmpty(request.UserName))
                    throw new Exception("Username is required.");
                else if (string.IsNullOrEmpty(request.PhoneNumber))
                    throw new Exception("Phone number is required.");

                var result = await _authService.RegisterAccount(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] StrateZone_Service.CustomModels.RequestModels.EmailLoginRequest loginRequest)
        {
            try
            {
                var result = await _authService.VerifyOTP(loginRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshToken(refreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
