using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;
using System.Security.Claims;

namespace StrateZone_APIs.Controllers
{ 
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IAppointmentService _appointmentService;
        private readonly IWalletService _walletService;
        private readonly ScheduleTimeValidator _scheduleTimeValidator;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IAppointmentService appointmentService, IWalletService walletService, ScheduleTimeValidator scheduleTimeValidator)
        {
            _paymentService = paymentService;
            _logger = logger;
            _appointmentService = appointmentService;
            _walletService = walletService;
            _scheduleTimeValidator = scheduleTimeValidator;
        }

        [HttpPost("booking-payment")]
        public async Task<IActionResult> ConfirmBookingPayment(AppointmentRequest request)
        {
            try
            {
                var userWallet = await _walletService.GetWalletByUserIdAsync(request.UserId);
                if(userWallet.Balance < request.TotalPrice )
                {
                    return StatusCode(500, "Balance is not enough");
                }

                foreach (var tb in request.TablesAppointmentRequests)
                {
                    var (isValid, errorMessage) = await _scheduleTimeValidator.IsScheduleTimeValid(tb.ScheduleTime, tb.EndTime, false);
                    if (!isValid) return BadRequest(new { message = errorMessage });
                }

                var unavailableTables = await _appointmentService.CheckAppointmentAvailability(request);

                if (unavailableTables.Count > 0)
                {
                    var errorResponse = new
                    {
                        error = new
                        {
                            code = "TABLE_NOT_AVAILABLE",
                            message = "Some tables are not available",
                            unavailable_tables = unavailableTables.Select(t => new
                            {
                                table_id = t.TableId,
                                start_time = t.ScheduleTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                                end_time = t.EndTime.ToString("yyyy-MM-ddTHH:mm:ss")
                            })
                        }
                    };

                    return new JsonResult(errorResponse)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var createdAppointment = await _appointmentService.CreateAppointmentAsync(request);
                var result = await _paymentService.CreatePaymentBooking(createdAppointment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("booking-request-payment")]
        public async Task<IActionResult> ConfirmBookingRequestPayment(TableAppointmentPaymentRequest request)
        {
            try
            {
                var result = await _paymentService.CreateAppointmentRequestPaymentBooking(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentModel payment)
        {
            try
            {
                var payments = await _paymentService.CreatePaymentAsync(payment);
                return Created("Payment created!", payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(PaymentParameters parameters)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsAsync(parameters);

                var response = new PagedListResponse<PaymentModel>(payments);

                return response != null ? Ok(response) : NotFound("No payment was found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tables-appointment/{id}")]
        public async Task<IActionResult> GetByTablesAppointment(int id)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByTablesAppointmentIdAsync(id);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetByUser(int id, PaymentParameters parameters)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByUserIdAsync(id, parameters);

                var response = new PagedListResponse<PaymentModel>(payments);

                return response != null ? Ok(response) : NotFound("No payment of this user was found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] PaymentModel payment, int id)
        {
            try
            {
                var payments = await _paymentService.UpdatePaymentAsync(payment, id);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
