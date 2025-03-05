using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [Route("api/ghn")]
    [ApiController]
    public class GHNController : ControllerBase
    {
        private readonly IGHNService _ghnService;

        public GHNController(IGHNService ghnService)
        {
            _ghnService = ghnService;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _ghnService.GetProvincesAsync();
            return Ok(result);
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var result = await _ghnService.GetServicesAsync();
            return Ok(result);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] object orderData)
        {
            var result = await _ghnService.CreateOrderAsync(orderData);
            return Ok(result);
        }
    }
}
