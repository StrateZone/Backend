using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System.Security.Claims;

namespace StrateZone_APIs.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPaymentService _paymentService;

        public PaymentController(IAppointmentService appointmentService,
            IPaymentService paymentService)
        {
            _appointmentService = appointmentService;
            _paymentService = paymentService;
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
    }
}
