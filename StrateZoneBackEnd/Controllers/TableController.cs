using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Utils;
using System.ComponentModel.DataAnnotations;

namespace StrateZone_APIs.Controllers
{
    [Route("api/tables")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        private readonly ILogger<TableController> _logger;
        private readonly ScheduleTimeValidator _scheduleTimeValidator;

        public TableController(ITableService tableService, ILogger<TableController> logger, ScheduleTimeValidator scheduleTimeValidator)
        {
            _tableService = tableService;
            _logger = logger;
            _scheduleTimeValidator = scheduleTimeValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TablesAppointmentParameters parameters, [FromQuery] string? search)
        {
            try
            {
                var result = await _tableService.GetAllTablesAsync(parameters, search);
                var response = new PagedListResponse<TableModel>(result);

                return response.TotalCount > 0 ? Ok(response) : Ok("No table was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetTables([FromQuery] TableParameters parameters)
        {
            try
            {
                var tables = await _tableService.GetTablesAsync(parameters);

                var response = new PagedListResponse<TableModel>(tables);

                return response.TotalCount > 0 ? Ok(response) : Ok("No table was found.");
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
                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(parameters, false);
                if (!isValid) return BadRequest(new { message = errorMessage });

                var tables = await _tableService.GetAvailableTablesAsync(parameters);

                var response = new PagedListResponse<TableResponse>(tables);

                return response.TotalCount > 0 ? Ok(response) : Ok("No available table was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTableById([FromQuery] DateTime StartTime, [FromQuery] DateTime EndTime, int id)
        {
            try
            {
                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(StartTime, EndTime, false);
                if (!isValid) return BadRequest(new { message = errorMessage });

                var table = await _tableService.GetTableByIdAsync(StartTime, EndTime, id);
                return table != null ? Ok(table) : NotFound("No table was found with this ID.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("gametype")]
        public async Task<IActionResult> GetTablesByGameType([FromQuery] TableParameters parameters, string gameType)
        {
            try
            {
                var tables = await _tableService.GetTablesByGameTypeAsync(parameters, gameType);

                var response = new PagedListResponse<TableModel>(tables);

                return response.TotalCount > 0 ? Ok(response) : Ok("No table was found for this gametype.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("available/gametype")]
        public async Task<IActionResult> GetAvailableTablesByGameType([FromQuery] TableParameters parameters, string gameType)
        {
            try
            {
                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(parameters, false);
                if (!isValid) return BadRequest(new { message = errorMessage });

                var tables = await _tableService.GetAvailableTablesByGameTypeAsync(parameters, gameType);

                var response = new PagedListResponse<TableResponse>(tables);

                return response.TotalCount > 0 ? Ok(response) : Ok("No available table was found for this gametype.");
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
        public async Task<IActionResult> GetAvailableTablesByGameTypeAndRoomType([FromQuery] TableParameters parameters, [FromQuery] string[] gameTypes, [FromQuery] string[] roomTypes)
        {
            try
            {
                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(parameters, false);
                if (!isValid) return BadRequest(new { message = errorMessage });

                var tables = await _tableService.GetAvailableTableByGameTypesAndRoomTypesInTimeRangeAsync(parameters, gameTypes, roomTypes);

                var response = new PagedListResponse<TableResponse>(tables);

                return response.TotalCount  > 0 ? Ok(response) : Ok("No available table was found for this gametype and roomtype.");
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
                var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(parameters, false);
                if (!isValid) return BadRequest(new { message = errorMessage });

                var tables = await _tableService.GetAvailableTablesForEachGameTypeInTimeRangeAsync(parameters, tableCount);
                return tables.Count > 0 ? Ok(tables) : Ok("No available table was found for this gametype and roomtype.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("available/monthly")]
        public async Task<IActionResult> GetAvailableTablesForMonthlyBookingType([FromQuery] TableMonthlyRequest parameters)
        {
            try
            {
                var tables = await _tableService.GetTablesWithinASpecificTimeRangeInMonthAsync(parameters.Year, parameters.Month, parameters.dayOfWeek, parameters.StartTime, parameters.EndTime, parameters.RoomType, parameters.GameType);
                return tables.DatesAndTables.Count > 0 ? Ok(tables) : Ok("No available table was found for this gametype and roomtype.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
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
        [Authorize(Policy = "AdminOnly")]
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

        [HttpPut("disable/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DisableTable(int id)
        {
            try
            {
                var table = await _tableService.DisableTableAsync(id);
                return Ok(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("enable/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EnableTable(int id)
        {
            try
            {
                var table = await _tableService.EnableTableAsync(id);
                return Ok(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
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
