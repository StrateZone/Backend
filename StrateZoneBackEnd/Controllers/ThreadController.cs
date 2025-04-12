using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using static StrateZone_Repository.Parameters.PostgreEnums;
using StrateZone_Service.CustomModels.ResponseModels;

namespace StrateZone_API.Controllers
{
    [ApiController]
    [Route("api/threads")]
    public class ThreadsController : ControllerBase
    {
        private readonly IThreadService _threadService;

        public ThreadsController(IThreadService threadService)
        {
            _threadService = threadService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateThread([FromBody] ThreadRequest request)
        {
            try
            {
                var result = await _threadService.CreateThreadAsync(request);
                return CreatedAtAction(nameof(GetThreadById), new { id = result.ThreadId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetThreadById(int id)
        {
            try
            {
                var result = await _threadService.GetThreadByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllThreads([FromQuery] TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _threadService.GetAllThreadsAsync(parameters);
                var response = new PagedListResponse<ThreadModel>(result);

                return response != null ? Ok(response) : Ok("No thread was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("filter/statuses")]
        public async Task<ActionResult> GetThreadsByStatuses(
            [FromQuery] TablesAppointmentParameters parameters,
            [FromQuery] ThreadStatus[] statuses)
        {
            try
            {
                var result = await _threadService.GetAllThreadsByStatusesAsync(parameters, statuses);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("filter/statuses-and-tags")]
        public async Task<ActionResult> GetThreadsByStatusesAndTags(
            [FromQuery] TablesAppointmentParameters parameters,
            [FromQuery] ThreadStatus[] statuses, 
            [FromQuery] HashSet<int> TagIds, 
            [FromQuery] int? userId)
        {
            try
            {
                var result = await _threadService.GetAllThreadsByStatusesAndTagsAsync(parameters, statuses, TagIds, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateThread(int id, [FromBody] ThreadModel model)
        {
            try
            {
                var result = await _threadService.UpdateThreadAsync(model, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteThread(int id)
        {
            try
            {
                var result = await _threadService.DeleteThreadAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("approve/{id}")]
        public async Task<ActionResult> ApproveThread(int id)
        {
            try
            {
                var result = await _threadService.ApproveThreadAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("reject/{id}")]
        public async Task<ActionResult> RejectThread(int id)
        {
            try
            {
                var result = await _threadService.RejectThreadAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
