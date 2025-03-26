using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
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
    
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IAppointmentService appointmentService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _appointmentService = appointmentService;
        }

        [HttpPost("booking-payment")]
        public async Task<ApiResponse<AppointmentModel>> ConfirmBookingPayment(AppointmentRequest request)
        {
            try
            {
                var createdAppointment = await _appointmentService.CreateAppointmentAsync(request);
                var result = await _paymentService.CreatePaymentBooking(createdAppointment);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
        public async Task<IActionResult> GetByUser(int id)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByUserIdAsync(id);
                return Ok(payments);
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
