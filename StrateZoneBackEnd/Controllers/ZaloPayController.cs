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

        [Authorize]
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
        public IActionResult HandleCallback([FromBody] dynamic cbdata)
        {
            var result = new Dictionary<string, object>();
            var key2 = _configuration["ZaloPay:Key2"];

            Console.WriteLine($"Callback Received: {JsonConvert.SerializeObject(cbdata)}");

            try
            {
                var dataStr = Convert.ToString(cbdata["data"]);
                var reqMac = Convert.ToString(cbdata["mac"]);

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                Console.WriteLine("mac = {0}", mac);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)
                if (!reqMac.Equals(mac))
                {
                    // callback không hợp lệ
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                }
                else
                {
                    // thanh toán thành công
                    // merchant cập nhật trạng thái cho đơn hàng
                    var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                    Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                    result["return_code"] = 1;
                    result["return_message"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["return_message"] = ex.Message;
            }

            // thông báo kết quả cho ZaloPay server
            return Ok(result);
        }
    }
}
