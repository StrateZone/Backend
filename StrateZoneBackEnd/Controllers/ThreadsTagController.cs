using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrateZone_Api.Controllers
{
    [ApiController]
    [Route("api/threadstags")]
    public class ThreadsTagController : ControllerBase
    {
        private readonly IThreadsTagService _threadsTagService;

        public ThreadsTagController(IThreadsTagService threadsTagService)
        {
            _threadsTagService = threadsTagService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThreadsTagModel threadsTag)
        {
            try
            {
                var result = await _threadsTagService.CreateThreadsTagAsync(threadsTag);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] List<ThreadsTagModel> threadsTags)
        {
            try
            {
                var result = await _threadsTagService.CreateThreadsTagsAsync(threadsTags);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("bulk/thread/{threadId}")]
        public async Task<IActionResult> CreateFromThread([FromBody] ThreadsTagsRequest request, int threadId)
        {
            try
            {
                var result = await _threadsTagService.CreateThreadsTagsAsync(request.TagIds, threadId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _threadsTagService.DeleteThreadsTagAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] ThreadsTagModel threadsTag, int id)
        {
            try
            {
                var result = await _threadsTagService.UpdateThreadsTagAsync(threadsTag, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ThreadsTagsRequest
    {
        public HashSet<int> TagIds { get; set; }
    }
}
