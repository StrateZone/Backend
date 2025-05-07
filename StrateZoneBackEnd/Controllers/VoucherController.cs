using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Repository.Parameters;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    [Authorize]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly ILogger<VoucherController> _logger;

        public VoucherController(IVoucherService voucherService, ILogger<VoucherController> logger)
        {
            _voucherService = voucherService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(TablesAppointmentParameters parameters) 
        { 
            try
            {
                var result = await _voucherService.GetVouchersAsync(parameters);
                var response = new PagedListResponse<VoucherModel>(result);

                return response != null ? Ok(response) : NotFound("No voucher was found");
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
                var result = await _voucherService.GetByIdAsync(id);

                return result != null ? Ok(result) : NotFound("No voucher was found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("samples")]
        public async Task<IActionResult> GetSampleVouchers(TablesAppointmentParameters parameters)
        {
            try
            {
                var result = await _voucherService.GetSampleVouchersAsync(parameters);
                var response = new PagedListResponse<VoucherModel>(result);

                return response != null ? Ok(response) : NotFound("No voucher was found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("of-user/{userId}")]
        public async Task<IActionResult> GetExchangableVouchers(TablesAppointmentParameters parameters, int userId)
        {
            try
            {
                var result = await _voucherService.GetVouchersByUserIdAsync(parameters, userId);
                var response = new PagedListResponse<VoucherModel>(result);

                return response != null ? Ok(response) : NotFound("No voucher was found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create-sample")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Post([FromBody] SampleVoucherRequest voucherModel)
        {
            try
            {
                var result = await _voucherService.CreateSampleVoucherAsync(voucherModel);

                return Created("Voucher created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create-voucher")]
        public async Task<IActionResult> CreateVoucherFromSample([FromBody] UserVoucherRequest voucherModel)
        {
            try
            {
                var result = await _voucherService.CreateVoucherFromSampleAsync(voucherModel);

                return Created("Voucher created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Put([FromBody] VoucherModel voucherModel, int id)
        {
            try
            {
                var result = await _voucherService.UpdateVoucherAsync(voucherModel, id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _voucherService.DeleteAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
