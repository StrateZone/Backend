using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private IRoomService _roomService;
        private ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetRooms([FromQuery] RoomParameters parameters)
        {
            try
            {
                var result = await _roomService.GetRoomsAsync(parameters);

                var response = new PagedListResponse<RoomResponse>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No room was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-type")]
        public async Task<IActionResult> GetRoomsByType([FromQuery] RoomParameters parameters, RoomType roomType)
        {
            try
            {
                var result = await _roomService.GetRoomsByRoomTypeAsync(parameters, roomType);

                var response = new PagedListResponse<RoomResponse>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No room of this type was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            try
            {
                var result = await _roomService.GetRoomByIdAsync(id);
                return result != null ? Ok(result) : NotFound("No room was found with this ID.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomRequest request)
        {
            try
            {
                var result = await _roomService.CreateRoomAsync(request);
                return Created("Room created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom([FromBody] RoomModel model, int id)
        {
            try
            {
                var result = await _roomService.UpdateRoomAsync(model, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var result = await _roomService.DeleteRoomAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
