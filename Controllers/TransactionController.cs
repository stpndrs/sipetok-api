using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Services;
using sipetok_api.Utils;


namespace sipetok_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly PaymentService _paymentService;
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;


        public TransactionController(AppDbContext context, PaymentService paymentService, OrderService orderService,IMapper mapper)
        {
            dbContext = context; 
            _mapper = mapper;
            _paymentService = paymentService;
            _orderService = orderService;
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

        [HttpPost("pay/{id:int}")]
        public async Task<IActionResult> Pay(int id)
        {
            try
            {
                // 1. Panggil service dengan parameter "NEXT" (sesuai default atau logic bisnis Anda)
                var success = await _paymentService.UpdateStatus(id, "NEXT");

                if (!success)
                {
                    return BadRequest(new ResponData<string>
                    {
                        success = false,
                        message = "Gagal memperbarui status pembayaran. Pastikan ID benar atau transisi status valid."
                    });
                }

                // 2. Ambil data transaksi terbaru dari database untuk di-map
                var transaction = await dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefaultAsync(t => t.id == id);

                if (transaction != null &&
                transaction.Status == PaymentState.Success && _orderService.UpdateOrderStatus(transaction, OrderTrigger.PaymentSucceeded))
                {
                    await dbContext.SaveChangesAsync();
                }


                // 3. Mapping object Transaction ke TransactionRespon
                var result = _mapper.Map<TransactionRespon>(transaction);


                return Ok(new ResponData<TransactionRespon>
                {
                    success = true,
                    data = result,
                    message = "Status pembayaran berhasil diperbarui."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponData<string> { success = false, message = ex.Message });
            }
        }

        [HttpPost("cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                // 1. Panggil service dengan action "CANCEL"
                var success = await _paymentService.UpdateStatus(id, "CANCEL");

                if (!success)
                {
                    return BadRequest(new ResponData<string>
                    {
                        success = false,
                        message = "Transaksi tidak ditemukan atau tidak dapat dibatalkan pada status saat ini."
                    });
                }

                // 2. Ambil data terbaru
                var transaction = await dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefaultAsync(t => t.id == id);

                if (transaction != null && _orderService.UpdateOrderStatus(transaction, OrderTrigger.CancelledByCustomer))
                {
                    await dbContext.SaveChangesAsync();
                }


                return Ok(new ResponData<TransactionRespon>
                {
                    success = true,
                    data = _mapper.Map<TransactionRespon>(transaction),
                    message = "Transaksi telah berhasil dibatalkan."
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponData<string> { success = false, message = ex.Message });
            }
        }
    }
    
}