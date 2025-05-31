using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Post([FromBody] WalletModel walletModel)
        {
            try
            {
                var result = await _walletService.CreateWalletAsync(walletModel);
                return Created("Wallet created!", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _walletService.GetWalletByIdAsync(id);
                return Ok(result);
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
                var result = await _walletService.GetWalletByUserIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update([FromBody] WalletModel walletModel, int id)
        {
            try
            {
                var result = await _walletService.UpdateWalletAsync(walletModel, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("deposit/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Deposit([FromQuery] int amount, int id)
        {
            try
            {
                await _walletService.DepositWalletAsync(amount, id);
                var result = await _walletService.GetWalletByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("withdrawal/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Withdrawal([FromQuery] int amount, int id)
        {
            try
            {
                var result = await _walletService.WithdrawalWalletAsync(amount, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class BankRequest
    {
        [Required]
        public int Amount { get; set; }

        [Required]
        public string Message { get; set; } = "";
    }
}
