using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;
using System.Security.Claims;

namespace StrateZone_APIs.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentController> _logger;
        private readonly ScheduleTimeValidator _scheduleTimeValidator;

        public AppointmentController(IAppointmentService appointmentService, ILogger<AppointmentController> logger, ScheduleTimeValidator scheduleTimeValidator)
        {
            _appointmentService = appointmentService;
            _logger = logger;
            _scheduleTimeValidator = scheduleTimeValidator;
        }

        [Authorize(Policy = "StaffAndAbove")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAppointments([FromQuery] AppointmentParameters parameters)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsAsync(parameters);

                var response = new PagedListResponse<AppointmentResponse>(appointments);

                return response.TotalCount > 0 ? Ok(response) : Ok("No appointment was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "StaffAndAbove")]
        [HttpGet("all/admin")]
        public async Task<IActionResult> GetAllAppointments([FromQuery] AppointmentAdminParameters parameters)
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync(parameters);

                var response = new PagedListResponse<AppointmentResponse>(appointments);

                return response.TotalCount > 0 ? Ok(response) : Ok("No appointment was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "StaffAndAbove")]
        [HttpGet("all-monthly/admin")]
        public async Task<IActionResult> GetAllMonthlyAppointments([FromQuery] AppointmentAdminParameters parameters)
        {
            try
            {
                var appointments = await _appointmentService.GetAllMonthlyAppointmentsAsync(parameters);

                var response = new PagedListResponse<AppointmentResponse>(appointments);

                return response.TotalCount > 0 ? Ok(response) : Ok("No appointment was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [Authorize(Policy = "StaffAndAbove")]
        [HttpGet("all/checkin")]
        public async Task<IActionResult> GetAllCheckinAppointments([FromQuery] AppointmentAdminParameters parameters)
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsCheckinAsync(parameters);

                var response = new PagedListResponse<AppointmentResponse>(appointments);

                return response.TotalCount > 0 ? Ok(response) : Ok("No appointment was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "StaffAndAbove")]
        [HttpPut("cancel/admin")]
        public async Task<IActionResult> RefundAppointment100Async(int tableAppointmentId, int userId)
        {
            try
            {
                var tableAppointment = await _appointmentService.RefundAppointment100Async(tableAppointmentId, userId);
                return Ok(new {message = "Appointment refunded: ", data = tableAppointment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                return appointment != null ? Ok(appointment) : NotFound("No appointment was found with this ID.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetAppointmentByUserId(int userId, [FromQuery] AppointmentParameters parameters)
        {
            try
            {
                userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var appointments = await _appointmentService.GetAppointmentsByUserIdAsync(parameters, userId);

                var response = new PagedListResponse<AppointmentResponse>(appointments);

                return response != null ? Ok(response) : NotFound("No appointment was found for this user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckAppointmentAvailability([FromBody] AppointmentRequest request)
        {
            try
            {
                foreach (var tb in request.TablesAppointmentRequests)
                {
                    var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(tb.ScheduleTime, tb.EndTime, false);
                    if (!isValid) return BadRequest(new { message = errorMessage });
                }

                var result = await _appointmentService.CheckAppointmentAvailability(request);

                if (result.Count > 0)
                {
                    var errorResponse = new
                    {
                        error = new
                        {
                            code = "TABLE_NOT_AVAILABLE",
                            message = "Some tables are not available",
                            unavailable_tables = result.Select(t => new
                            {
                                table_id = t.TableId,
                                start_time = t.ScheduleTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                                end_time = t.EndTime.ToString("yyyy-MM-ddTHH:mm:ss")
                            })
                        }
                    };

                    return new JsonResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                return Ok("All requested tables for this appointment are available.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateAppointment([FromBody] AppointmentModel appointmentModel, int id)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentAsync(appointmentModel, id);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.DeleteAppointmentAsync(id);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
