using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
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
                return appointments.Count > 0 ? Ok(appointments) : Ok("No appointment.");
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
                var appointment = await _appointmentService.GetAppointmentsByUserIdAsync(parameters, userId);
                return appointment != null ? Ok(appointment) : NotFound("No appointment was found for this user.");
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
                if (!ScheduleTimeValidator.IsScheduleTimeValid(request, out string msg))
                    return BadRequest(new { message = msg });

                var result = await _appointmentService.CheckAppointmentAvailability(request);
                return result.Count > 0
                    ?
                    StatusCode(500, $"The following tables are not available: {string.Join(", ", result)}")
                    :
                    Ok("All requested tables for this appointment are available.");
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
                if (!ScheduleTimeValidator.IsScheduleTimeValid(request, out string msg))
                    return BadRequest(new { message = msg });

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
                if (!ScheduleTimeValidator.IsScheduleTimeValid(appointmentModel, out string msg))
                    return BadRequest(new { message = msg });

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
