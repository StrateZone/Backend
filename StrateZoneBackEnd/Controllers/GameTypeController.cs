using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [Route("api/game_types")]
    [ApiController]
    public class GameTypeController : ControllerBase
    {
        private readonly IGameTypeService _gameTypeService;
        private readonly ILogger<GameTypeController> _logger;

        public GameTypeController(IGameTypeService gameTypeService, ILogger<GameTypeController> logger)
        {
            _gameTypeService = gameTypeService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGameTypes()
        {
            try
            {
                var gametypes = await _gameTypeService.GetGameTypesAsync();
                return Ok(gametypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("all-extend")]
        public async Task<IActionResult> GetAllGameTypesWithExtensions()
        {
            try
            {
                var gametypes = await _gameTypeService.GetGameTypesWithExtensionsAsync();
                return Ok(gametypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-id")]
        public async Task<IActionResult> GetByTypeId(int id)
        {
            try
            {
                var gametypes = await _gameTypeService.GetGameTypeByIdAsync(id);
                return Ok(gametypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-id-extend")]
        public async Task<IActionResult> GetWithExtensionsByTypeId(int id)
        {
            try
            {
                var gametypes = await _gameTypeService.GetGameTypeWithExtensionsByIdAsync(id);
                return Ok(gametypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
