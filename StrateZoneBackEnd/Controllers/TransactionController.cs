using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(TransactionParameters parameters)
        {
            try
            {
                var result = await _transactionService.GetTransactionsAsync(parameters);
                var response = new PagedListResponse<TransactionModel>(result);

                return response != null ? Ok(response) : NotFound("No transaction was found.");
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
                var result = await _transactionService.GetById(id);

                return result != null ? Ok(result) : NotFound("No transaction was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetByUserId(int id, TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _transactionService.GetUserTransactionsAsync(id, parameters);
                var response = new PagedListResponse<TransactionModel>(result);

                return response != null ? Ok(response) : NotFound("No transaction was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
