using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/points-history")]
    public class PointsHistoryController : ControllerBase
    {
        private readonly IPointsHistoryService _pointsHistoryService;
        private readonly ILogger<PointsHistoryController> _logger;

        public PointsHistoryController(IPointsHistoryService pointsHistoryService, ILogger<PointsHistoryController> logger)
        {
            _pointsHistoryService = pointsHistoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _pointsHistoryService.GetAllAsync(parameters);
                var response = new PagedListResponse<PointsHistoryModel>(result);

                return result != null ? Ok(response) : NotFound("No points history found");
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
                var result = await _pointsHistoryService.GetByIdAsync(id);

                return result != null ? Ok(result) : NotFound("Points history not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("of-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId, [FromQuery] TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _pointsHistoryService.GetByUserIdAsync(userId, parameters);
                var response = new PagedListResponse<PointsHistoryModel>(result);

                return result != null ? Ok(response) : NotFound("Points history for user not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PointsHistoryModel model)
        {
            try
            {
                var result = await _pointsHistoryService.AddAsync(model);

                return Created("Points history created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] PointsHistoryModel model, int id)
        {
            try
            {
                await _pointsHistoryService.UpdateAsync(model, id);
                return Ok("Points history updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _pointsHistoryService.DeleteAsync(id);
                return Ok("Points history deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
