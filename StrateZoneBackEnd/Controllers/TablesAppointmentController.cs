using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
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
        public async Task<IActionResult> GetTablesAppointments(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _services.GetAllTablesAppointmentsAsync(parameters);

                var response = new PagedListResponse<TablesAppointmentResponse>(result);

                return response != null ? Ok(response) : NotFound("No tables_appointment was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetTablesAppointmentsByUserId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _services.GetAllTablesAppointmentByTableIdAsync(id, parameters);
                var response = new PagedListResponse<TablesAppointmentResponse>(result);

                return response != null ? Ok(response) : NotFound("No tables_appointment for this user ID was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tables/{id}")]
        public async Task<IActionResult> GetTablesAppointmentsByTableId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _services.GetAllTablesAppointmentByTableIdAsync(id, parameters);

                var response = new PagedListResponse<TablesAppointmentResponse>(result);

                return response != null ? Ok(response) : NotFound("No tables_appointment for this table ID was found.");
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

        [HttpGet("joined-by/users/{userId}")]
        public async Task<IActionResult> GetTablesAppointmentInvitedToUserByUserId(int userId, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _services.GetAllTablesAppointmentsJoinedByUserId(userId, parameters);
                var response = new PagedListResponse<TablesAppointmentResponse>(result);

                return response != null ? Ok(response) : NotFound("No tables_appointment joined by this user ID was found.");
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
                var result = await _services.CreateTablesAppointmentAsync(model);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTablesAppointment([FromBody] TablesAppointmentModel model, int id)
        {
            try
            {
                var result = await _services.UpdateTablesAppointmentAsync(model, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("check-in/{tablesAppointmentId}/users/{userId}")]
        public async Task<IActionResult> CheckinTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var result = await _services.CheckInTablesAppointment(tablesAppointmentId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("cancel-check/{tablesAppointmentId}/users/{userId}")]
        public async Task<IActionResult> CancelTablesAppointment(int tablesAppointmentId, int userId, DateTime CancelTime)
        {
            try
            {
                var result = await _services.CalculateRefundAmountOnAppointmentCancellation(userId, tablesAppointmentId, CancelTime);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("cancel/{tablesAppointmentId}/users/{userId}")]
        public async Task<IActionResult> CancelTablesAppointment(int tablesAppointmentId, int userId)
        {
            try
            {
                var result = await _services.CancelTablesAppointment(tablesAppointmentId, userId);
                return Ok(result);
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
