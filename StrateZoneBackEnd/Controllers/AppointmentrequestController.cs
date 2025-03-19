using MealHunt_Repositories.Pagination;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/appointmentrequests")] 
    
    public class AppointmentrequestController : ControllerBase
    {
        private readonly IAppointmentrequestService _appointmentrequestService;
        private readonly ILogger<AppointmentrequestController> _logger;

        public AppointmentrequestController(IAppointmentrequestService appointmentrequestService, ILogger<AppointmentrequestController> logger)
        {
            _appointmentrequestService = appointmentrequestService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentRequestByIdAsync(int id)
        {
            try
            {
                var result = await _appointmentrequestService.GetAppointmentRequestByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("from/{userId}")]
        public async Task<IActionResult> GetAppointmentRequestsFromUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentrequestService.GetAppointmentRequestsFromUserByUserIdAsync(parameters, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-appointment/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentRequestsByAppointmentIdAsync(AppointmentRequestParameters parameters, int appointmentId)
        {
            try
            {
                var result = await _appointmentrequestService.GetAppointmentRequestsByAppointmnetIdAsync(parameters, appointmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("to/{userId}")]
        public async Task<IActionResult> GetAppointmentRequestsOfUserByUserIdAsync(AppointmentRequestParameters parameters, int userId)
        {
            try
            {
                var result = await _appointmentrequestService.GetAppointmentRequestsOfUserByUserIdAsync(parameters, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointmentRequestAsync([FromBody] AppointmentrequestRequest request)
        {
            try
            {
                var result = await _appointmentrequestService.CreateAppointmentRequestAsync(request);
                return Created("Appointment request created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointmentRequestAsync([FromBody] AppointmentrequestModel appointmentRequestModel, int id)
        {
            try
            {
                var result = await _appointmentrequestService.UpdateAppointmentRequestAsync(appointmentRequestModel, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentRequestAsync(int id)
        {
            try
            {
                var result = await _appointmentrequestService.DeleteAppointmentRequestAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
