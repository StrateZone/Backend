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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWithExtensionsByTypeId(int id)
        {
            try
            {
                if (id <= 0) return BadRequest("Invalid ID");

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
