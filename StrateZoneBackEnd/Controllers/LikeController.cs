using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace StrateZone_API.Controllers
{
    [ApiController]
    [Route("api/likes")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost]
        public async Task<ActionResult<LikeModel>> CreateLike([FromBody] LikeRequest request)
        {
            try
            {
                var result = await _likeService.CreateLike(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<LikeModel>> DeleteLike(int id)
        {
            try
            {
                var result = await _likeService.DeleteLike(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Like with ID {id} not found." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
