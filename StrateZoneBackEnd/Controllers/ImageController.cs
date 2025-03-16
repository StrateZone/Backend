using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageService imageService, ILogger<ImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] ImageRequest request)
        {
            try
            {
                var result = await _imageService.CreateImageAsync(request);
                return Created("Image uploaded succesffuly!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("avatars/{user_id}")]
        public async Task<IActionResult> GetUserAvatar(int user_id)
        {
            try
            {
                var result = await _imageService.GetUserAvatarAsync(user_id);
                return result != null ? Ok(result) : NotFound("No avatar for this user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tournaments/{tournament_id}")]
        public async Task<IActionResult> GetTournamentThumbnail(int tournament_id)
        {
            try
            {
                var result = await _imageService.GetTournamentThumbnailAsync(tournament_id);
                return result != null ? Ok(result) : NotFound("No thumbnail for this tournament was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("events/{event_id}")]
        public async Task<IActionResult> GetEventThumbnail(int event_id)
        {
            try
            {
                var result = await _imageService.GetEventThumbnailAsync(event_id);
                return result != null ? Ok(result) : NotFound("No thumbnail for this event was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("products/{product_id}")]
        public async Task<IActionResult> GetProductImages(int product_id)
        {
            try
            {
                var result = await _imageService.GetProductImagesAsync(product_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("gametypes/{type_id}")]
        public async Task<IActionResult> GetGameTypeImage(int type_id)
        {
            try
            {
                var result = await _imageService.GetGametypeThumbnail(type_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("threads/{thread_id}")]
        public async Task<IActionResult> GetThreadImages(int thread_id)
        {
            try
            {
                var result = await _imageService.GetThreadImagesAsync(thread_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var result = await _imageService.DeleteImageAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
