using Microsoft.AspNetCore.Mvc;
using static StrateZone_Repository.Parameters.PostgreEnums;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize(Policy = "AdminOnly")]
    public class AnalyticController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly IThreadService _threadService;
        private readonly ITablesAppointmentService _tablesAppointmentService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<TransactionController> _logger;

        public AnalyticController(ITransactionService transactionService, ILogger<TransactionController> logger, IUserService userService, IThreadService threadService, ITablesAppointmentService tablesAppointmentService, IPaymentService paymentService)
        {
            _transactionService = transactionService;
            _logger = logger;
            _userService = userService;
            _threadService = threadService;
            _tablesAppointmentService = tablesAppointmentService;
            _paymentService = paymentService;
        }

        [HttpGet("new-users/year/{year}/month/{month}")]
        public async Task<IActionResult> GetNewUsersInAMonth(int month, int year)
        {
            try
            {
                var result = await _userService.GetUsersJoinedInAMonth(month, year);

                return result != null ? Ok(result) : NotFound("No user was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tables-appointment-report/year/{year}/month/{month}")]
        public async Task<IActionResult> GetReportForTAGroupedByMonth(int month, int year)
        {
            try
            {
                var result = await _tablesAppointmentService.GetAllBookedTablesAppointmentWithinAMonthInYearAsync(month, year);

                return result != null ? Ok(result) : NotFound("No transaction was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("memberships-purchased/year/{year}/month/{month}")]
        public async Task<IActionResult> GetMembershipInAMonth(int month, int year)
        {
            try
            {
                var result = await _paymentService.GetMembershipPaymentsWithinAMonthInYearAsync(month, year);

                return result > 0 ? Ok(result) : NotFound("No membership purchased was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("threads-report/year/{year}/month/{month}")]
        public async Task<IActionResult> GetThreadsInAMonth(int month, int year)
        {
            try
            {
                var result = await _threadService.GetAllThreadsWithinAMonthInYearAsync(month, year);

                return result != null ? Ok(result) : NotFound("No membership purchased was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("transaction-report/year/{year}/month/{month}")]
        public async Task<IActionResult> GetReportForTransactionsGroupedByMonth(int month, int year)
        {
            try
            {
                var result = await _transactionService.GetDailyTransactionReportsInAMonth(month, year);

                return result != null ? Ok(result) : NotFound("No transaction was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("profit/year/{year}/month/{month}")]
        public async Task<IActionResult> GetProfitForEachMonth(int month, int year)
        {
            try
            {
                var result = await _transactionService.GetDailyProfitInAMonth(month, year);

                return result != null ? Ok(result) : NotFound("No transaction was found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
