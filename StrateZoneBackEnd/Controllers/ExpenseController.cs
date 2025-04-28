using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/expenses")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpenseService expenseService, ILogger<ExpenseController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _expenseService.GetExpensesAsync(parameters);
                var response = new PagedListResponse<ExpenseModel>(result);

                return response != null ? Ok(response) : NotFound("No expenses found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _expenseService.GetByIdAsync(id);

                return result != null ? Ok(result) : NotFound($"Expense with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ExpenseRequest expense)
        {
            try
            {
                var result = await _expenseService.AddAsync(expense);

                return Created("Expense created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("add-multiple")]
        public async Task<IActionResult> PostMultiple([FromBody] List<ExpenseRequest> expenses)
        {
            try
            {
                var result = await _expenseService.AddRangeAsync(expenses);

                return Created("Expenses created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] ExpenseModel expenseModel, int id)
        {
            try
            {
                var result = await _expenseService.UpdateAsync(expenseModel, id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _expenseService.DeleteAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
