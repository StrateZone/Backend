using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [ApiController]
    [Route("api/wallets")]
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
}
