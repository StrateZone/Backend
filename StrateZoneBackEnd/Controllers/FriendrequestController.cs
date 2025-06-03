using StrateZone_Repository.Pagination;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/friendrequests")]
    [Authorize]
    //[Authorize(Policy = "ClubMember")]
    public class FriendrequestController : ControllerBase
    {
        private readonly IFriendrequestService _friendrequestService;
        private readonly ILogger<FriendrequestController> _logger;

        public FriendrequestController(IFriendrequestService friendrequestService, ILogger<FriendrequestController> logger)
        {
            _friendrequestService = friendrequestService;
            _logger = logger;
        }


        [HttpGet("to/{id}")]
        public async Task<IActionResult> GetFriendrequestsOfUserIdAsync(FriendrequestParameters parameters, int id)
        {
            try
            {
                var result = await _friendrequestService.GetFriendrequestsOfUserIdAsync(parameters, id);
                var response = new PagedListResponse<FriendrequestModel>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No friend request to this user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("from/{id}")]
        public async Task<IActionResult> GetFriendrequestsFromUserIdAsync(FriendrequestParameters parameters, int id)
        {
            try
            {
                var result = await _friendrequestService.GetFriendrequestsFromUserIdAsync(parameters, id);
                var response = new PagedListResponse<FriendrequestModel>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No friend request to this user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFriendrequestByIdAsync(int id)
        {
            try
            {
                var result = await _friendrequestService.GetFriendrequestByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFriendrequestAsync([FromBody] FriendrequestRequest request)
        {
            try
            {
                var result = await _friendrequestService.CreateFriendrequestAsync(request);
                return Created("Friendrequest created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFriendrequestAsync([FromBody] FriendrequestModel friendrequestModel, int id)
        {
            try
            {
                var result = await _friendrequestService.UpdateFriendrequestAsync(friendrequestModel, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("accept/{id}")]
        public async Task<IActionResult> AcceptFriendrequestAsync(int id)
        {
            try
            {
                var result = await _friendrequestService.AcceptFriendrequestAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectFriendrequestAsync(int id)
        {
            try
            {
                var result = await _friendrequestService.RejectFriendrequestAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendrequestAsync(int id)
        {
            try
            {
                var result = await _friendrequestService.DeleteFriendrequestAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("sender/{senderId}/receiver/{receiverId}")]
        public async Task<IActionResult> DeleteFriendrequestBySenderAndReceiverAsync(int senderId, int receiverId)
        {
            try
            {
                var result = await _friendrequestService.DeleteFriendrequestByUserAndFriendIdAsync(senderId, receiverId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
