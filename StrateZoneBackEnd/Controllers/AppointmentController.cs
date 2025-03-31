using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;

namespace StrateZone_APIs.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService appointmentService, ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAppointments([FromQuery] AppointmentParameters parameters)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsAsync(parameters);

                var response = new PagedListResponse<AppointmentModel>(appointments);

                return response.TotalCount > 0 ? Ok(response) : Ok("No appointment was found.");
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
                var appointments = await _appointmentService.GetAppointmentsByUserIdAsync(parameters, userId);

                var response = new PagedListResponse<AppointmentModel>(appointments);

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
                    if (!ScheduleTimeValidator.IsScheduleTimeValid(tb.ScheduleTime, tb.EndTime, false, out string msg))
                        return BadRequest(new { message = msg });
                }

                var result = await _appointmentService.CheckAppointmentAvailability(request);

                if (result.Count > 0)
                {
                    var tableInfo = result
                        .Select(t => $"TableId: {t.TableId}, ScheduleTime: {t.ScheduleTime}, EndTime: {t.EndTime}")
                        .ToList();

                    return StatusCode(500, $"The following tables are not available:\n{string.Join("\n", tableInfo)}");
                }

                return Ok("All requested tables for this appointment are available.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAppointment([FromBody] StrateZone_Service.CustomModels.RequestModels.AppointmentRequest request)
        {
            try
            {
                foreach (var tb in request.TablesAppointmentRequests)
                {
                    if (!ScheduleTimeValidator.IsScheduleTimeValid(tb.ScheduleTime, tb.EndTime, false, out string msg))
                        return BadRequest(new { message = msg });
                }

                var appointment = await _appointmentService.CreateAppointmentAsync(request);
                return Created("Appointment created", appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment([FromBody] AppointmentModel appointmentModel, int id)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentAsync(appointmentModel, id);
                return Ok("Appointment updated:\n" + appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
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
