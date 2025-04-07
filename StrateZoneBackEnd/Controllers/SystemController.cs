using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;
        private readonly ILogger<SystemController> _logger;
    
        public SystemController(ISystemService systemService, ILogger<SystemController> logger)
        {
            _systemService = systemService;
            _logger = logger;
        }

        [HttpGet("{id}/open-hour")]
        public async Task<IActionResult> GetOpenHour(int id)
        {
            try
            {
                var result = await _systemService.GetOpeningHourAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/close-hour")]
        public async Task<IActionResult> GetCloseHour(int id)
        {
            try
            {
                var result = await _systemService.GetClosingHourAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/open-hour/date")]
        public async Task<IActionResult> GetOpenHour(int id, DateOnly date)
        {
            try
            {
                var result = await _systemService.GetOpeningHourOnDateAsync(id, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/close-hour/date")]
        public async Task<IActionResult> GetCloseHour(int id, DateOnly date)
        {
            try
            {
                var result = await _systemService.GetClosingHourOnDateAsync(id, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("working-hour/{id}")]
        public async Task<IActionResult> UpdateWorkingHour(int id, [FromBody] SystemWorkingHourUpdateRequest request)
        {
            try
            {
                if (request.CloseHour < request.OpenHour)
                    return BadRequest("Open hour must be sooner than close hour.");

                var result = await _systemService.UpdateSystemWorkingTimeAsync(id, request.OpenHour, request.CloseHour);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSystem(int id)
        {
            try
            {
                var result = await _systemService.GetSystemsByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/abnormal-days")]
        public async Task<IActionResult> GetAbnormalDays(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _systemService.GetAbnormalDaysAsync(id, parameters);
                var response = new PagedListResponse<AbnormalDayModel>(result);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("abnormal-day")]
        public async Task<IActionResult> AddDay([FromBody] AbnormalDayRequest request)
        {
            try
            {
                var result = await _systemService.AddAbnormalDayAsync(request);
                return Created("Abnormal day added!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("abnormal-day/{id}")]
        public async Task<IActionResult> UpdateAbnormalDay(int id, [FromBody] AbnormalDayModel model)
        {
            try
            {
                var result = await _systemService.UpdateAbnormalDayAsync(model, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("abnormal-day/{id}")]
        public async Task<IActionResult> DeleteAbnormalDay(int id)
        {
            try
            {
                var result = await _systemService.DeleteAbnormalDayAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
