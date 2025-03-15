using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [Route("api/tables-appointment")]
    [ApiController] 
    public class TablesAppointmentController : ControllerBase
    {
        private readonly ITablesAppointmentService _services;
        private readonly ILogger<TablesAppointmentController> _logger;

        public TablesAppointmentController(ITablesAppointmentService services, ILogger<TablesAppointmentController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetTablesAppointments()
        {
            try
            {
                var result = await _services.GetAllTablesAppointmentsAsync();
                return result.Count > 0 ? Ok(result) : Ok("No tables_appointment was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tables/{id}")]
        public async Task<IActionResult> GetTablesAppointmentsByTableId(int id)
        {
            try
            {
                var result = await _services.GetAllTablesAppointmentByTableIdAsync(id);
                return result.Count > 0 ? Ok(result) : Ok("No tables_appointment for this table ID was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("appointments/{id}")]
        public async Task<IActionResult> GetTablesAppointmentsByAppointmentId(int id)
        {
            try
            {
                var result = await _services.GetAllTablesAppointmentByAppointmentIdAsync(id);
                return result.Count > 0 ? Ok(result) : Ok("No tables_appointment for this appointment ID was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tables/{tableId}/appointments/{appointmentId}")]
        public async Task<IActionResult> GetTablesAppointmentByTableIdAndAppointmentId(int tableId, int appointmentId)
        {
            try
            {
                var result = await _services.GetTablesAppointmentByTableIdAndAppointmentIdAsync(tableId, appointmentId);
                return result != null ? Ok(result) : Ok("No tables_appointment for this table ID was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTablesAppointment([FromBody] TablesAppointmentModel model)
        {
            try
            {
                var result = await _services.CreateTablesAppointment(model);
                return Created("tables_appointment created", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("from-appointment")]
        public async Task<IActionResult> CreateTablesAppointmentFromAppointment([FromBody] AppointmentModel model)
        {
            try
            {
                var result = await _services.CreateTablesAppointmentsFromAppointmentAsync(model);
                return Created("tables_appointment created", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTablesAppointment(int id)
        {
            try
            {
                var result = await _services.DeleteTablesAppointmentAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
