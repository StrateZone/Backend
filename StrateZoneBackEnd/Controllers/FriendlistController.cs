using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/friendlists")]
 //   [Authorize(Policy = "ClubMember")]
    public class FriendlistController : ControllerBase
    {
        private readonly IFriendlistService _friendlistService;
        private readonly ILogger<FriendlistController> _logger;

        public FriendlistController(IFriendlistService friendlistService, ILogger<FriendlistController> logger)
        {
            _friendlistService = friendlistService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFriendsByUserIdAsync([FromQuery] TablesAppointmentParameters parameters, int userId)
        {
            try
            {
                var result = await _friendlistService.GetFriendsByUserIdAsync(parameters, userId);
                var response = new PagedListResponse<FriendlistResponse>(result);

                return response != null ? Ok(response) : Ok("No friends found for this user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving friend list for user {UserId}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFriendByIdAsync(int id)
        {
            try
            {
                var result = await _friendlistService.GetFriendByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Friend with ID {id} not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving friend by ID {FriendId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFriendAsync([FromBody] FriendlistModel friendModel)
        {
            try
            {
                var result = await _friendlistService.AddFriendAsync(friendModel);
                return Created("Friend added successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding friend");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFriendAsync([FromBody] FriendlistModel friendModel, int id)
        {
            try
            {
                var result = await _friendlistService.UpdateFriendAsync(friendModel, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating friend with ID {FriendId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendAsync(int id)
        {
            try
            {
                var result = await _friendlistService.DeleteFriendAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting friend with ID {FriendId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
