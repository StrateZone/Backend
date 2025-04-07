using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;
    
        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetByUserId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _notificationService.GetUserNotificationsAsync(id, parameters);
                var response = new PagedListResponse<NotificationModel>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No notification sent to this user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _notificationService.GetByIdAsync(id);

                return result != null ? Ok(result) : NotFound("No notification with this ID was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationRequest request)
        {
            try
            {
                var result = await _notificationService.CreateNotificationAsync(request);

                return Created("Notification created", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
