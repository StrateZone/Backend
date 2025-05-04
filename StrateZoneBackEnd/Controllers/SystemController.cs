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

        [HttpGet("{id}/check-in/minutes-before-schedule")]
        public async Task<IActionResult> GetSystemCheckinHours(int id)
        {
            try
            {
                var result = await _systemService.GetAppointmentCheckinTimeInMinuesAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/refund-100/hours-before-schedule")]
        public async Task<IActionResult> GetSystemRefundHours(int id)
        {
            try
            {
                var result = await _systemService.GetAppointmentRefund100TimeInHoursAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/appointment-requests/hours-until-expiration")]
        public async Task<IActionResult> GetAppointmentRequestsTimes(int id)
        {
            try
            {
                var result = await _systemService.GetSystemsByIdAsync(id);
                return Ok(new AppointmentRequestTimeRules() { AppointmentRequests_MaxHours_UntilExpiration = result.AppointmentRequest_MaxHours_UntilExpiration, AppointmentRequests_MinHours_UntilExpiration = result.AppointmentRequest_MinHours_UntilExpiration });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/appointment-requests/max-invitations-to-table")]
        public async Task<IActionResult> GetMaxUsersInvitedToTable(int id)
        {
            try
            {
                var result = await _systemService.GetMaxUsersInvitedToTable(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/top-contributors/numbers-per-week")]
        public async Task<IActionResult> GetNumberOfTopContributionsPerThread(int id)
        {
            try
            {
                var result = await _systemService.GetNumberOfTopContributionsPerThread(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/incoming/hours-before-schedule")]
        public async Task<IActionResult> GetSystemInComingHours(int id)
        {
            try
            {
                var result = await _systemService.GetAppointmentIncomingTimeInHoursAsync(id);
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

        [HttpPut("{id}/appointment-time-rules")]
        public async Task<IActionResult> UpdateTimeRules(int id, [FromBody] AppointmentTimeRules model)
        {
            try
            {
                var result = await _systemService.UpdateAppointmentTimeRulesAsync(id, model.Refund100_Hours_BeforeScheduleTime, model.Incoming_Hours_BeforeScheduleTime, model.Checkin_Minutes_BeforeScheduleTime, model.MaxTablesCancel_PerWeek);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/points-rules")]
        public async Task<IActionResult> UpdateTimeRules(int id, [FromBody] PointRules model)
        {
            try
            {
                var result = await _systemService.UpdatePointsRulesAsync(id, model.UserPoints_By_TablePricePercentage, model.ContributionPoints_PerThread, model.ContributionPoints_PerComment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> General(int id, [FromBody] SystemModel model)
        {
            try
            {
                var result = await _systemService.UpdateSystemAsync(id, model);
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

    public class AppointmentTimeRules
    {
        public decimal Refund100_Hours_BeforeScheduleTime { get; set; }
        public decimal Incoming_Hours_BeforeScheduleTime { get; set; }
        public int Checkin_Minutes_BeforeScheduleTime { get; set; }
        public int MaxTablesCancel_PerWeek { get; set; }
    }

    public class PointRules
    {
        public float UserPoints_By_TablePricePercentage { get; set; }
        public int ContributionPoints_PerThread { get; set; }
        public int ContributionPoints_PerComment { get; set; }
    }

    public class AppointmentRequestTimeRules
    {
        public float AppointmentRequests_MaxHours_UntilExpiration { get; set; }
        public float AppointmentRequests_MinHours_UntilExpiration { get; set; }
    }
}
