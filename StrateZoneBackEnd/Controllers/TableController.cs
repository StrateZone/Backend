using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [Route("api/tables")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        private readonly ILogger<TableController> _logger;

        public TableController(ITableService tableService, ILogger<TableController> logger)
        {
            _tableService = tableService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetTables()
        {
            try
            {
                var tables = await _tableService.GetTablesAsync();
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-id")]
        public async Task<IActionResult> GetTableById(int id)
        {
            try
            {
                var table = await _tableService.GetTableByIdAsync(id);
                return Ok(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-game-type")]
        public async Task<IActionResult> GetTablesByGameType(StrateZone_Repository.Parameters.PostgreEnums.GameType gameType)
        {
            try
            {
                var tables = await _tableService.GetTablesByGameTypeAsync(gameType);
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTable([FromBody] TableRequest request)
        {
            try
            {
                var table = await _tableService.CreateTableAsync(request);
                return Ok(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTable(int id)
        {
            try
            {
                var table = await _tableService.DeleteTableAsync(id);
                return Ok(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
