using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/prices")]
    public class PriceController : ControllerBase
    {
        private readonly IPriceService _priceService;
        private readonly ILogger<PriceController> _logger;

        public PriceController(IPriceService priceService, ILogger<PriceController> logger)
        {
            _priceService = priceService;
            _logger = logger;
        }

        /**
         * Retrieve the prices for roomtypes and gametypes 
         */
        [HttpGet("services")]
        public async Task<IActionResult> GetServicePrices([FromQuery] PriceParameters parameters) 
        {
            try
            {
                var result = await _priceService.GetServicePricesAsync(parameters);

                var response = new PagedListResponse<PriceModel>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No price was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("roomtype")]
        public async Task<IActionResult> GetPriceByRoomType(RoomType roomType)
        {
            try
            {
                var result = await _priceService.GetPriceOfRoomTypeAsync(roomType);
                return result != null ? Ok(result) : NotFound("Price for this room type doesn't exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("gametype")]
        public async Task<IActionResult> GetPriceByGameType(GameTypeEnum gameType)
        {
            try
            {
                var result = await _priceService.GetPriceOfGameTypeAsync(gameType);
                return result != null ? Ok(result) : NotFound("Price for this game type doesn't exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("membership")]
        public async Task<IActionResult> GetMembershipFee()
        {
            try
            {
                var result = await _priceService.GetMembershipPriceAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("salary")]
        public async Task<IActionResult> GetSalary()
        {
            try
            {
                var result = await _priceService.GetTeachingSalaryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductPrice(int id)
        {
            try
            {
                var result = await _priceService.GetProductPriceByIdAsync(id);
                return result != null ? Ok(result) : NotFound("Price for this product doesn't exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("appointments/{id}")]
        public async Task<IActionResult> GetAppointmentPrice(int id)
        {
            try
            {
                var result = await _priceService.GetPriceOfAppointmentAsync(id);
                return result != null ? Ok(result) : NotFound("Price for this appointment doesn't exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentPrice([FromQuery] int[] tableIds, [FromQuery] DateTime StartTime, [FromQuery] DateTime EndTime)
        {
            try
            {
                var result = await _priceService.GetPriceOfAppointmentFromAppointmentRequestAsync(tableIds, StartTime, EndTime);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrice([FromBody] PriceModel model, int id)
        {
            try
            {
                var result = await _priceService.UpdatePriceAsync(model, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
