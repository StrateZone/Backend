using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> 
    }
}
