using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StrateZone_APIs.Controllers
{
    //[Route("api/payment")]
    //[ApiController]
    //public class PaymentController : ControllerBase
    //{

    //    public PaymentController()
    //    {

    //    }

    //    public async Task<IActionResult> CreatePaymentUrl(CreatePaymentLinkRequest request)
    //    {
    //        try
    //        {
    //            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    //            if (userIdClaim == null)
    //            {
    //                return Unauthorized();
    //            }
    //            var userId = Int32.Parse(userIdClaim.Value.Trim());
    //            request.UserId = userId;
    //            var response = await _paymentService.CreatePaymentUrl(request);
    //            return Ok(response);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex);
    //            return StatusCode(500, ex.Message);
    //        }
    //    }
    //}
}
