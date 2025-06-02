using StrateZone_Repository.Pagination;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/appointmentrequests")]
    [Authorize]
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
                userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _appointmentrequestService.GetAppointmentRequestsFromUserByUserIdAsync(parameters, userId);
                
                var response = new PagedListResponse<AppointmentrequestModel>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No appointment request from this user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{userId}/tables_appointments/{tableAppointmentId}")]
        public async Task<IActionResult> GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(int userId, int tableAppointmentId)
        {
            try
            {
                var result = await _appointmentrequestService.GetAppointmentRequestsFromUserByUserAndTablesAppointmentIdAsync(userId, tableAppointmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{userId}/tables/{tableId}")]
        public async Task<IActionResult> GetCurrentAppointmentRequestsFromUserByUserAndTablesIdAsync(int userId, int tableId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var result = await _appointmentrequestService.GetCurrentAppointmentRequestsFromUserByUserAndTableIdAsync(userId, tableId, startTime, endTime);
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
                userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _appointmentrequestService.GetAppointmentRequestsOfUserByUserIdAsync(parameters, userId);
                
                var response = new PagedListResponse<AppointmentrequestModel>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No appointment request of this user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointmentRequestsAsync([FromBody] AppointmentrequestsRequest request)
        {
            try
            {
                request.FromUser = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                List<AppointmentrequestRequest> requests = new();

                foreach (var toUserId in request.ToUser)
                {
                    AppointmentrequestRequest appointmentrequestRequest = new AppointmentrequestRequest()
                    { 
                        FromUser = request.FromUser,
                        ToUser = toUserId,
                        AppointmentId = request.AppointmentId,
                        TableId = request.TableId,
                        StartTime = request.StartTime,
                        EndTime = request.EndTime,
                        TotalPrice = request.TotalPrice,
                    };

                    requests.Add(appointmentrequestRequest);
                }

                var result = await _appointmentrequestService.CreateAdditionalAppointmentRequestsAsync(requests);
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

        [HttpPut("accept/{id}")]
        public async Task<IActionResult> AcceptAppointmentRequestAsync(int id)
        {
            try
            {
                var result = await _appointmentrequestService.AcceptAppointmentrequestAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectAppointmentRequestAsync(int id)
        {
            try
            {
                var result = await _appointmentrequestService.RejectAppointmentrequestAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("cancel-all/users/{id}")]
        public async Task<IActionResult> CancelAllAppointmentRequestAsync(int id)
        {
            try
            {
                var result = await _appointmentrequestService.CancelAllSentRequestFromUserAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("cancel-all/users/{userId}/tables/{tableId}")]
        public async Task<IActionResult> CancelAllAppointmentRequestAsync(int userId, int tableId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var result = await _appointmentrequestService.CancelAllAppointmentRequestsFromUserOnTableAsync(userId, tableId, startTime, endTime);
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
