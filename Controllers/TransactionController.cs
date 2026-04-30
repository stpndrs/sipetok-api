using Microsoft.AspNetCore.Mvc;
using sipetok_api.service; 
using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
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
            var success = await _paymentService.UpdateStatus(id, "NEXT");
            if (success) 
                return Ok(new {
                message = "Status berhasil diperbarui." });

            return BadRequest(new { message = "Gagal memproses." });
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _paymentService.UpdateStatus(id, "CANCEL");
            if (success) 
                return Ok(new {
                message = "Transaksi telah dibatalkan." });

            return BadRequest(new { message = "Transaksi tidak bisa dibatalkan." });
        }
    }
    
}