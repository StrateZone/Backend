using Azure.Core;
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

        [HttpGet("all/available")]
        public async Task<IActionResult> GetAvailableTables()
        {
            try
            {
                var tables = await _tableService.GetAvailableTablesAsync();
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
                return table != null ? Ok(table) : NotFound("No table was found with this ID.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-game-type")]
        public async Task<IActionResult> GetTablesByGameType(StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                var tables = await _tableService.GetTablesByGameTypeAsync(gameType);
                return tables.Count > 0 ? Ok(tables) : Ok("No table was found for this gametype.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-game-type/available")]
        public async Task<IActionResult> GetAvailableTablesByGameType(StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                var tables = await _tableService.GetAvailableTablesByGameTypeAsync(gameType);
                return tables.Count > 0 ? Ok(tables) : Ok("No table available was found for this gametype.");
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
                return Created("Table created", table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTable([FromBody] TableModel tableModel, int id)
        {
            try
            {
                var table = await _tableService.UpdateTableAsync(tableModel, id);
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
