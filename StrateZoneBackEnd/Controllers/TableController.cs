using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;
using static StrateZone_Repository.Parameters.PostgreEnums;

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
        public async Task<IActionResult> GetTables([FromQuery] TableParameters parameters)
        {
            try
            {
                var tables = await _tableService.GetTablesAsync(parameters);
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("available/all")]
        public async Task<IActionResult> GetAvailableTables([FromQuery] TableParameters parameters)
        {
            try
            {
                if (!ScheduleTimeValidator.IsScheduleTimeValid(parameters, out string msg))
                    return BadRequest(new { message = msg });

                var tables = await _tableService.GetAvailableTablesAsync(parameters);
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
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

        [HttpGet("gametype")]
        public async Task<IActionResult> GetTablesByGameType([FromQuery] TableParameters parameters, StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                var tables = await _tableService.GetTablesByGameTypeAsync(parameters, gameType);
                return tables.Count > 0 ? Ok(tables) : Ok("No table was found for this gametype.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("available/gametype")]
        public async Task<IActionResult> GetAvailableTablesByGameType([FromQuery] TableParameters parameters, StrateZone_Repository.Parameters.PostgreEnums.GameTypeEnum gameType)
        {
            try
            {
                if (!ScheduleTimeValidator.IsScheduleTimeValid(parameters, out string msg))
                    return BadRequest(new { message = msg });

                var tables = await _tableService.GetAvailableTablesByGameTypeAsync(parameters, gameType);
                return tables.Count > 0 ? Ok(tables) : Ok("No available table was found for this gametype.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get available tables within a time range, filtered by gametypes and roomtypes.
        /// </summary>
        [HttpGet("available/filter")]
        public async Task<IActionResult> GetAvailableTablesByGameTypeAndRoomType([FromQuery] TableParameters parameters, [FromQuery] GameTypeEnum[] gameTypes, [FromQuery] RoomType[] roomTypes)
        {
            try
            {
                if (!ScheduleTimeValidator.IsScheduleTimeValid(parameters, true, out string msg))
                    return BadRequest(new { message = msg });

                var tables = await _tableService.GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(parameters, gameTypes, roomTypes);
                return tables.Count > 0 ? Ok(tables) : Ok("No available table was found for this gametype and roomtype.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get X available tables for each game type, where X is the value of <param name="tableCount">tableCount</param>.
        /// </summary>
        [HttpGet("available/each")]
        public async Task<IActionResult> GetAvailableTablesForEachGameType([FromQuery] TableParameters parameters, [FromQuery] int tableCount)
        {
            try
            {
                if (!ScheduleTimeValidator.IsScheduleTimeValid(parameters, out string msg))
                    return BadRequest(new { message = msg });

                var tables = await _tableService.GetAvailableTablesForEachGameTypeInTimeRangeAsync(parameters, tableCount);
                return tables.Count > 0 ? Ok(tables) : Ok("No available table was found for this gametype and roomtype.");
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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
