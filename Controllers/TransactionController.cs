using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Services;
using sipetok_api.Models;
using sipetok_api.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using sipetok_api.Utils;
using sipetok_api.Repositories;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;
        private readonly PaymentService _paymentService;
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;

        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");

        public TransactionController(TransactionFactory factory, AppDbContext context, PaymentService paymentService, OrderService orderService, IMapper mapper)
        {
            _factory = factory;
            _dbContext = context;
            _mapper = mapper;
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(CurrentUserId);
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"TenantId : {tenant.Id}" };

            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Transaction, TransactionResponseDto>(
                new Transaction(), new TransactionResponseDto(), null, CurrentUserId, searchQuery, new[] { "Details", "Details.Category" });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(CurrentUserId);
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"TenantId : {tenant.Id}" };

            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Transaction, TransactionResponseDto>(
                new Transaction(), new TransactionResponseDto(), id, CurrentUserId, searchQuery, new[] { "Details", "Details.Category" });
        }

        [HttpPost]
        public async Task<IActionResult> Store([FromBody] TransactionRequestDto request)
        {
            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(CurrentUserId);
            if (tenant == null) return Forbid();

            using var transactionScope = await _dbContext.Database.BeginTransactionAsync();
            request.TenantId = tenant.Id;

            try
            {
                var transactionData = new Transaction
                {
                    Date = request.Date,
                    PaymentAmount = 0,
                    TotalPrice = 0,
                    PaymentStatus = PaymentState.WaitingForPayment,
                    OrderStatus = OrderState.OrderComeIn,
                    CustomerName = request.CustomerName,
                    CustomerPhoneNumber = request.CustomerPhoneNumber,
                };

                var worker = _factory.CreateMethod("save");
                var result = await worker.ActionAsync<Transaction, TransactionResponseDto, Transaction>(
                    new Transaction(), new TransactionResponseDto(), transactionData, "POST");

                if (result is not OkObjectResult okResult || okResult.Value is not Transaction createdTransaction)
                {
                    return BadRequest("Gagal menyimpan transaksi utama.");
                }

                if (request.Details != null && request.Details.Any())
                {
                    double totalCalculated = 0;

                    foreach (var d in request.Details)
                    {
                        var eggCategory = await _dbContext.EggCategories.FindAsync(d.CategoryId);
                        if (eggCategory == null) throw new Exception($"Kategori telur ID {d.CategoryId} tidak ditemukan.");

                        double subtotal = (double)d.Quantity * eggCategory.Price;
                        totalCalculated += subtotal;

                        _dbContext.TransactionDetails.Add(new TransactionDetail
                        {
                            TransactionId = createdTransaction.Id,
                            CategoryId = d.CategoryId,
                            Quantity = d.Quantity,
                            Subtotal = subtotal,
                            PriceAtPurchase = eggCategory.Price
                        });
                    }

                    createdTransaction.TotalPrice = totalCalculated;
                    _dbContext.Transactions.Update(createdTransaction);
                    await _dbContext.SaveChangesAsync();
                }

                await transactionScope.CommitAsync();

                var responseData = _mapper.Map<TransactionResponseDto>(createdTransaction);

                return Ok(new { success = true, message = "Berhasil menambahkan transaksi (Orderan Masuk & Menunggu Pembayaran)", data = responseData });
            }
            catch (Exception ex)
            {
                await transactionScope.RollbackAsync();
                return BadRequest(new { Message = "Terjadi kesalahan", Error = ex.Message });
            }
        }

        [HttpPost("pay/{id:int}")]
        public async Task<IActionResult> Pay(int id, [FromBody] PaymentRequestDto paymentDto)
        {
            try
            {
                var success = await _paymentService.UpdateStatus(id, PaymentTrigger.Pay, paymentDto);
                if (!success) return BadRequest(new { success = false, message = "Gagal memproses pembayaran. Pastikan ID benar atau status saat ini valid." });

                return await GetTransactionResponseAsync(id, "Pembayaran sukses dicatat, stok telur berhasil dikurangi, dan pesanan SIAP DIAMBIL.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var success = await _paymentService.UpdateStatus(id, PaymentTrigger.Cancel, null);
                if (!success) return BadRequest(new { success = false, message = "Transaksi tidak ditemukan atau tidak dapat dibatalkan pada status saat ini." });

                return await GetTransactionResponseAsync(id, "Transaksi dan pesanan telah berhasil dibatalkan.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("complete/{id:int}")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            try
            {
                var transaction = await FetchTransactionWithDetailsAsync(id);
                if (transaction == null) return NotFound(new { success = false, message = "Transaksi tidak ditemukan." });

                var isUpdated = _orderService.UpdateOrderStatus(transaction, OrderTrigger.PickedUp);
                if (!isUpdated) return BadRequest(new { success = false, message = "Gagal menyelesaikan pesanan. Pastikan status pesanan saat ini adalah 'ReadyForPickup'." });

                await _dbContext.SaveChangesAsync();

                return Ok(new { success = true, message = "Pesanan selesai! Telur telah diambil oleh pelanggan.", data = _mapper.Map<TransactionResponseDto>(transaction) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private async Task<Transaction?> FetchTransactionWithDetailsAsync(int id)
        {
            return await _dbContext.Transactions
                .Include(t => t.Details)
                .ThenInclude(d => d.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        private async Task<IActionResult> GetTransactionResponseAsync(int id, string successMessage)
        {
            var transaction = await FetchTransactionWithDetailsAsync(id);
            return Ok(new { success = true, message = successMessage, data = _mapper.Map<TransactionResponseDto>(transaction) });
        }
    }
}