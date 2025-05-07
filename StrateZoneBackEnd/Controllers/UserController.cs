using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_APIs.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("all")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserListParameters parameters)
        {
            try
            {
                var user = await _userService.GetUsersAsync(parameters);

                var response = new PagedListResponse<UserResponse>(user);

                return response.TotalCount > 0 ? Ok(response) : Ok("No user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("all/management")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsersManagement([FromQuery] UserListManagementParameters parameters)
        {
            try
            {
                var user = await _userService.GetUsersManagementAsync(parameters);

                var response = new PagedListResponse<UserManagementResponse>(user);

                return response.TotalCount > 0 ? Ok(response) : Ok("No user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("all/dashboard")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsersDashboard()
        {
            try
            {
                var user = await _userService.GetUsersDashboardAsync();

                return user.Count > 0 ? Ok(user) : Ok("No user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("username/search")]
        public async Task<IActionResult> GetByUsername([FromQuery] UserListParameters parameters, string username)
        {
            try
            {
                var user = await _userService.GetUsersByUsernameAsync(parameters, username);

                var response = new PagedListResponse<UserResponse>(user);

                return response.TotalCount > 0 ? Ok(response) : Ok("No user with this username was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/search-friends")]
        public async Task<IActionResult> SearchForFriendsUsername([FromQuery] UserListParameters parameters, int id, string? username)
        {
            try
            {
                var user = await _userService.SearchForFriendsByUsernameAsync(parameters, id, username);

                var response = new PagedListResponse<FriendResponse>(user);

                return response.TotalCount > 0 ? Ok(response) : Ok("No user with this username was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-ranking")]
        public async Task<IActionResult> GetByRanking([FromQuery] UserListParameters parameters, Ranking ranking, int up, int down)
        {
            try
            {
                var user = await _userService.GetUsersByRankingAsync(parameters, ranking, up, down);
                var response = new PagedListResponse<UserResponse>(user);
                return response != null ? Ok(response) : Ok("No user of this ranking was found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("opponents/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandomByRanking(int userId, [FromQuery] string? SearchTerm, [FromQuery] HashSet<int> excludedIds)
        {
            try
            {
                var opponenents = await _userService.GetRandomOpponentsAsync(userId, SearchTerm, excludedIds);
                return opponenents.MatchingOpponents.Count > 0 ? Ok(opponenents) : NotFound("No user was found");
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
                var user = await _userService.GetUserByIdAsync(id);
                return user != null ? Ok(user) : NotFound("No user with this ID was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("phone/{phone}")]
        public async Task<IActionResult> GetByPhone(string phone)
        {
            try
            {
                var user = await _userService.GetUserByPhoneNumberAsync(phone);
                return user != null ? Ok(user) : NotFound("No user with this phone number was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddUser([FromBody] UserRequest user)
        {
            try
            {
                var result = await _userService.CreateUserAsync(user);
                return Created("User added", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser([FromBody] UserModel user, int id)
        {
            try
            {
                var result = await _userService.EditUserProfileAsync(user, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel user, int id)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(user, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("suspend/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SuspendUser(int userId)
        {
            try
            {
                var result = await _userService.SuspendUserAccount(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("kick/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> KickUser(int userId)
        {
            try
            {
                var result = await _userService.KickUserFromCommunityAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("password/{userId}")]
        public async Task<IActionResult> ChangePasswordAsync(int userId, [FromBody] UserPasswordRequest userPasswordRequest)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(userId, userPasswordRequest.OldPassword, userPasswordRequest.NewPassword, userPasswordRequest.ConfirmPassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("forgot-password/{userId}")]
        public async Task<IActionResult> ChangeForgotPasswordAsync(int userId, [FromBody] UserForgotPasswordRequest userPasswordRequest)
        {
            try
            {
                var result = await _userService.ForgotPasswordAsync(userId, userPasswordRequest.NewPassword, userPasswordRequest.ConfirmPassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("check-notification/{id}")]
        public async Task<IActionResult> CheckUserNotification(int id)
        {
            try
            {
                var result = await _userService.CheckUserNotification(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("points/{id}")]
        public async Task<IActionResult> GetUserPointsAsync(int id)
        {
            try
            {
                var result = await _userService.GetUserPointsAsync(id);
                return Ok(new UserPointsResponse() { Points = result.Item1, ContributionPoints = result.Item2 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("auto-assign-top-contributors")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AssignTopContributors()
        {
            try
            {
                await _userService.AssignTopContributorsAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class UserPointsResponse
    {
        public int Points { get; set; }
        public int ContributionPoints { get; set; }
    }
}
