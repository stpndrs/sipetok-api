using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Services;
using sipetok_api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sipetok_api.Models;
using sipetok_api.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var worker = _factory.CreateMethod("get");
            Transaction transactionModel = new Transaction();
            TransactionResponseDto response = new TransactionResponseDto();

            return await worker.ActionAsync<Transaction, TransactionResponseDto>(transactionModel, response, null, userId, null, new[] { "Details", "Details.Category" });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var worker = _factory.CreateMethod("get");
            Transaction transactionModel = new Transaction();
            TransactionResponseDto response = new TransactionResponseDto();

            return await worker.ActionAsync<Transaction, TransactionResponseDto>(transactionModel, response, id, userId, null, new[] { "Details", "Details.Category" });
        }

        [HttpPost]
        public async Task<IActionResult> Store([FromBody] TransactionRequestDto request)
        {
            // Menggunakan IDbContextTransaction agar aman (Atomic)
            // find tenant by userId
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            Tenant tenant = await _dbContext.Tenants.FirstOrDefaultAsync(a => a.UserId == userId);
            using var transactionScope = await _dbContext.Database.BeginTransactionAsync();
            request.TenantId = tenant!.Id;
            try
            {
                // 1. Siapkan data transaksi dasar
                var transactionData = new Transaction
                {
                    Date = request.Date,
                    PaymentAmount = 0,
                    TotalPrice = 0, // Akan dihitung nanti
                    TenantId = request.TenantId,
                    PaymentStatus = PaymentState.WaitingForPayment,
                    OrderStatus = OrderState.OrderComeIn,
                    CustomerName = request.CustomerName,
                    CustomerPhoneNumber = request.CustomerPhoneNumber,
                };

                // 2. Simpan transaksi utama lewat worker
                IStevanMethod worker = _factory.CreateMethod("save");
                var result = await worker.ActionAsync<Transaction, TransactionResponseDto, Transaction>(
                    new Transaction(), new TransactionResponseDto(), transactionData, "POST");

                // 3. Pastikan transaksi utama berhasil tersimpan dan kita dapatkan ID-nya
                if (!(result is OkObjectResult okResult && okResult.Value is Transaction createdTransaction))
                {
                    return BadRequest("Gagal menyimpan transaksi utama.");
                }

                // 4. Proses Details jika ada
                if (request.Details != null && request.Details.Any())
                {
                    double totalCalculated = 0;

                    foreach (var d in request.Details)
                    {
                        // Ambil harga dari database
                        var eggCategory = await _dbContext.EggCategories.FindAsync(d.CategoryId);
                        if (eggCategory == null) throw new Exception($"Kategori telur ID {d.CategoryId} tidak ditemukan.");

                        double subtotal = (double)d.Quantity * eggCategory.Price;
                        totalCalculated += subtotal;

                        // Tambahkan detail ke entitas yang sudah tersimpan
                        _dbContext.TransactionDetails.Add(new TransactionDetail
                        {
                            TransactionId = createdTransaction.Id, // Menggunakan ID hasil dari ActionAsync
                            CategoryId = d.CategoryId,
                            Quantity = d.Quantity,
                            Subtotal = subtotal,
                            PriceAtPurchase = eggCategory.Price
                        });
                    }

                    // Update total price pada transaksi utama
                    createdTransaction.TotalPrice = totalCalculated;
                    _dbContext.Transactions.Update(createdTransaction);

                    // Simpan semua detail
                    await _dbContext.SaveChangesAsync();
                }

                // 5. Commit semua perubahan
                await transactionScope.CommitAsync();

                // Susun respon sesuai format yang kamu minta
                var responseData = new
                {
                    id = createdTransaction.Id,
                    date = createdTransaction.Date,
                    paymentAmount = createdTransaction.PaymentAmount,
                    totalPrice = createdTransaction.TotalPrice,
                    tenantId = createdTransaction.TenantId,
                    customerName = createdTransaction.CustomerName,
                    customerPhoneNumber = createdTransaction.CustomerPhoneNumber,
                    status = createdTransaction.PaymentStatus.ToString(), // Pastikan Enum jadi string
                    orderStatus = createdTransaction.OrderStatus.ToString(),
                    details = createdTransaction.Details.Select(d => new
                    {
                        id = d.Id,
                        transactionId = d.TransactionId,
                        categoryName = d.Category?.Name ?? "Unknown", // Pastikan ada navigasi ke EggCategory
                        quantity = d.Quantity,
                        price = d.PriceAtPurchase,
                        subtotal = d.Subtotal
                    }).ToList()
                };

                return Ok(new
                {
                    success = true,
                    message = "Berhasil menambahkan transaksi (Orderan Masuk & Menunggu Pembayaran)",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transactionScope.RollbackAsync();
                return BadRequest(new { Message = "Terjadi kesalahan", Error = ex.Message });
            }
        }

        [HttpPost("pay/{id:int}")]
        public async Task<IActionResult> Pay(int id, [FromBody] PaymentDto paymentDto)
        {
            try
            {
                var success = await _paymentService.UpdateStatus(id, PaymentTrigger.Pay, paymentDto);
                if (!success)
                    return BadRequest(new { success = false, message = "Gagal memproses pembayaran. Pastikan ID benar atau status saat ini valid." });

                var transaction = await _dbContext.Transactions.Include(t => t.Details).ThenInclude(d => d.Category).FirstOrDefaultAsync(t => t.Id == id);

                return Ok(new
                {
                    success = true,
                    message = "Pembayaran sukses dicatat, stok telur berhasil dikurangi, dan pesanan SIAP DIAMBIL.",
                    data = _mapper.Map<TransactionResponseDto>(transaction)
                });
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
                if (!success)
                    return BadRequest(new { success = false, message = "Transaksi tidak ditemukan atau tidak dapat dibatalkan pada status saat ini." });

                var transaction = await _dbContext.Transactions.Include(t => t.Details).ThenInclude(d => d.Category).FirstOrDefaultAsync(t => t.Id == id);

                return Ok(new
                {
                    success = true,
                    message = "Transaksi dan pesanan telah berhasil dibatalkan.",
                    data = _mapper.Map<TransactionResponseDto>(transaction)
                });
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
                var transaction = await _dbContext.Transactions.Include(t => t.Details).ThenInclude(d => d.Category).FirstOrDefaultAsync(t => t.Id == id);
                if (transaction == null)
                    return NotFound(new { success = false, message = "Transaksi tidak ditemukan." });

                var isUpdated = _orderService.UpdateOrderStatus(transaction, OrderTrigger.PickedUp);
                if (!isUpdated)
                    return BadRequest(new { success = false, message = "Gagal menyelesaikan pesanan. Pastikan status pesanan saat ini adalah 'ReadyForPickup'." });

                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Pesanan selesai! Telur telah diambil oleh pelanggan.",
                    data = _mapper.Map<TransactionResponseDto>(transaction)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}