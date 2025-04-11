using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTagsAsync()
        {
            try
            {
                var result = await _tagService.GetTagsAsync();
                return result.Count > 0 ? Ok(result) : Ok("No tags found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tags.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagByIdAsync(int id)
        {
            try
            {
                var result = await _tagService.GetTagByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Tag with ID {id} not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag with ID {TagId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTagsAsync([FromQuery] string content)
        {
            try
            {
                var result = await _tagService.SearchTagsAsync(content);
                return result.Count > 0 ? Ok(result) : Ok("No matching tags found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tags with content '{Content}'", content);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTagAsync([FromBody] TagModel tagModel)
        {
            try
            {
                var result = await _tagService.CreateTagAsync(tagModel);
                return Created("Tag created successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTagAsync(int id)
        {
            try
            {
                var result = await _tagService.DeleteTagAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag with ID {TagId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
