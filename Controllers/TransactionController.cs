using Microsoft.AspNetCore.Mvc;
using sipetok_api.service; 
using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        
        public TransactionController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pay/{id}")]
        public async Task<IActionResult> Pay(int id)
        {
            
            var success = await _paymentService.UpdateStatus(id);

            if (success)
            {
                return Ok(new
                {
                    message = "Pembayaran Berhasil!",
                    status = "Lunas"
                });
            }

            return BadRequest(new
            {
                message = "Gagal memproses pembayaran. Cek apakah transaksi ada atau sudah lunas."
            });
        }
    }
}