using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using ZaloPay.Helper.Crypto;

namespace StrateZone_APIs.Controllers
{
    [Route("api/zalo-pay")]
    [ApiController]
    public class ZaloPayController : ControllerBase
    {
        private readonly IZaloPayService _zaloPayService;
        private readonly IConfiguration _configuration;

        public ZaloPayController(IZaloPayService zaloPayService, IConfiguration configuration)
        {
            _zaloPayService = zaloPayService;
            _configuration = configuration;
        }

        //[Authorize]
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] ZaloPayRequest request)
        {
            try
            {
                var response = await _zaloPayService.CreatePaymentRequestAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("callback")]
        public async Task<IActionResult> HandleCallback([FromBody] dynamic cbdata)
        {
            try
            {
                var response = await _zaloPayService.HandleCallbackAsync(cbdata);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
