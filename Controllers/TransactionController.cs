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

        public TransactionController(AppDbContext context, PaymentService paymentService, OrderService orderService, IMapper mapper)
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

                var respon = new ResponData<List<TransactionRespon>>(true, result, "Berhasil mengambil semua data transaksi");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Store(TransactionDto transactionDto)
        {
            try
            {
                var transaction = await _paymentService.ProcessTransaction(transactionDto);

                var completeData = dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefault(t => t.id == transaction.id);

                var respon = new ResponData<TransactionRespon>(true, _mapper.Map<TransactionRespon>(completeData), "Berhasil menambahkan transaksi (Orderan Masuk & Menunggu Pembayaran)");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPost("pay/{id:int}")]
        public async Task<IActionResult> Pay(int id)
        {
            try
            {
                var success = await _paymentService.UpdateStatus(id, PaymentTrigger.Pay, _orderService, OrderTrigger.PaymentSucceeded);

                if (!success)
                {
                    return BadRequest(new ResponData<object?>(false, "Gagal memproses pembayaran. Pastikan ID benar atau status saat ini valid."));
                }

                var transaction = await dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefaultAsync(t => t.id == id);

                var result = _mapper.Map<TransactionRespon>(transaction);

                var respon = new ResponData<TransactionRespon>(true, result, "Pembayaran sukses dicatat, stok telur berhasil dikurangi, dan pesanan SIAP DIAMBIL.");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPost("cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var success = await _paymentService.UpdateStatus(id, PaymentTrigger.Cancel, _orderService, OrderTrigger.CancelledByCustomer);

                if (!success)
                {
                    return BadRequest(new ResponData<object?>(false, "Transaksi tidak ditemukan atau tidak dapat dibatalkan pada status saat ini."));
                }

                var transaction = await dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefaultAsync(t => t.id == id);

                var respon = new ResponData<TransactionRespon>(true, _mapper.Map<TransactionRespon>(transaction), "Transaksi dan pesanan telah berhasil dibatalkan.");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPost("complete/{id:int}")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            try
            {
                var transaction = await dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefaultAsync(t => t.id == id);

                if (transaction == null)
                {
                    return NotFound(new ResponData<object?>(false, "Transaksi tidak ditemukan."));
                }

                var isUpdated = _orderService.UpdateOrderStatus(transaction, OrderTrigger.PickedUp);

                if (!isUpdated)
                {
                    return BadRequest(new ResponData<object?>(false, "Gagal menyelesaikan pesanan. Pastikan status pesanan saat ini adalah 'ReadyForPickup' (Siap Diambil)."));
                }

                await dbContext.SaveChangesAsync();

                var respon = new ResponData<TransactionRespon>(true, _mapper.Map<TransactionRespon>(transaction), "Pesanan selesai! Telur telah diambil oleh pelanggan.");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }
    }
}