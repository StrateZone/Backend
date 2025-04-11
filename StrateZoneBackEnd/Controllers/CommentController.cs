using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrateZone_API.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            try
            {
                var result = await _commentService.GetCommentById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("thread/{threadId}")]
        public async Task<IActionResult> GetCommentsByThreadId(int threadId)
        {
            try
            {
                var result = await _commentService.GetCommentsByThreadIdAsync(threadId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCommentsByUserId(int userId)
        {
            try
            {
                var result = await _commentService.GetCommentsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] CommentRequest request)
        {
            try
            {
                var result = await _commentService.PostCommentAsync(request);
                return CreatedAtAction(nameof(GetCommentById), new { id = result.CommentId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentModel comment)
        {
            try
            {
                var result = await _commentService.UpdateCommentAsync(comment, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var result = await _commentService.DeleteCommentAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
