using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.service; 
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly PaymentService _paymentService;
        private readonly IMapper _mapper;

        public TransactionController(AppDbContext context, PaymentService paymentService, IMapper mapper)
        {
            dbContext = context; 
            _mapper = mapper;
            _paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var data = dbContext.Transactions
                    .Include(t => t.details)
                    .ToList();

                var result = _mapper.Map<List<TransactionRespon>>(data);

                return Ok(new ResponData<List<TransactionRespon>>
                {
                    success = true,
                    data = result,
                    message = "Berhasil mengambil semua data transaksi"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponData<string> { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Store(TransactionDto transactionDto)
        {
            try
            {
                var transaction = await _paymentService.ProcessTransaction(transactionDto);

                // Ambil data lengkap untuk respon (include details)
                var completeData = dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefault(t => t.id == transaction.id);

                return Ok(new ResponData<TransactionRespon>
                {
                    success = true,
                    data = _mapper.Map<TransactionRespon>(completeData),
                    message = "Berhasil menambahkan transaksi"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponData<string> { success = false, message = ex.Message });
            }
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