using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sipetok_api.Controllers.Products
{
    public class GetData
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetData(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IActionResult> ActionAsync<TEntity, TResponse>(string subAction, int? id = null, int? userId = null) where TEntity : class
        {
            try
            {
                string entityName = typeof(TEntity).Name;
                string action = subAction?.ToLower()?.Trim() ?? string.Empty;

                // Menggunakan Switch Expression untuk routing yang lebih bersih
                return action switch
                {
                    "getall" => await HandleGetAllAsync<TEntity, TResponse>(entityName),
                    "byid" => await HandleGetByIdAsync<TEntity, TResponse>(id, entityName),

                    // --- Spesifik Modul ---
                    "get_my_tenant" => await HandleGetMyTenantAsync<TResponse>(userId),
                    "get_my_user" => await HandleGetMyUserAsync<TResponse>(userId),

                    "get_tx_by_id" => await HandleGetTransactionByIdAsync<TResponse>(id, userId),
                    "tx_all_tenant" => await HandleGetTransactionsByTenantAsync<TResponse>(userId),
                    "op_all_tenant" => await HandleGetAllOperationalByTenantAsync<TResponse>(userId),
                    "category_all_tenant" => await HandleGetAllCategoryByTenantAsync<TResponse>(userId),
                    "egg_all_tenant" => await HandleGetAllEggByTenantAsync<TResponse>(userId),

                    _ => new BadRequestObjectResult(new ResponData<object>(false, $"Sub-action '{subAction}' tidak dikenali."))
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ResponData<object>(false, ex.Message)) { StatusCode = 500 };
            }
        }

        #region HANDLERS
        private async Task<IActionResult> HandleGetAllAsync<TEntity, TResponse>(string entityName) where TEntity : class
        {
            var records = await _dbContext.Set<TEntity>().ToListAsync();
            return new OkObjectResult(new ResponData<List<TResponse>>(true, _mapper.Map<List<TResponse>>(records), $"Berhasil mengambil data {entityName}"));
        }

        private async Task<IActionResult> HandleGetByIdAsync<TEntity, TResponse>(int? id, string entityName) where TEntity : class
        {
            var record = await _dbContext.Set<TEntity>().FindAsync(id ?? 0);
            if (record is null) return new NotFoundObjectResult(new ResponData<object>(false, $"{entityName} dengan ID {id} tidak ditemukan"));

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(record), $"Berhasil mengambil {entityName}"));
        }

        private async Task<IActionResult> HandleGetAllOperationalByTenantAsync<TResponse>(int? userId)
        {
            var data = await _dbContext.Operationals
                .Include(o => o.Tenant)
                .Where(o => o.Tenant!.UserId == (userId ?? 0))
                .ToListAsync();

            return new OkObjectResult(new ResponData<List<TResponse>>(true, _mapper.Map<List<TResponse>>(data), "Berhasil mengambil data operasional tenant"));
        }

        private async Task<IActionResult> HandleGetAllCategoryByTenantAsync<TResponse>(int? userId)
        {
            var data = await _dbContext.EggCategories
                .Include(c => c.Tenant)
                .Where(c => c.Tenant!.UserId == (userId ?? 0))
                .ToListAsync();

            return new OkObjectResult(new ResponData<List<TResponse>>(true, _mapper.Map<List<TResponse>>(data), "Berhasil mengambil data kategori telur tenant"));
        }

        private async Task<IActionResult> HandleGetAllEggByTenantAsync<TResponse>(int? userId)
        {
            var data = await _dbContext.Eggs
                .Include(e => e.Category).ThenInclude(c => c!.Tenant)
                .Where(e => e.Category!.Tenant!.UserId == (userId ?? 0))
                .ToListAsync();

            return new OkObjectResult(new ResponData<List<TResponse>>(true, _mapper.Map<List<TResponse>>(data), "Berhasil mengambil data telur tenant"));
        }

        private async Task<IActionResult> HandleGetMyTenantAsync<TResponse>(int? userId)
        {
            var tenant = await _dbContext.Tenants
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == (userId ?? 0));

            if (tenant is null) return new NotFoundObjectResult(new ResponData<object>(false, "Profil Tenant tidak ditemukan"));

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(tenant), "Berhasil mengambil profil tenant"));
        }

        private async Task<IActionResult> HandleGetMyUserAsync<TResponse>(int? userId)
        {
            var user = await _dbContext.Users.FindAsync(userId ?? 0);
            if (user is null) return new NotFoundObjectResult(new ResponData<object>(false, "Profil user not found"));

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(user), "Successfully retrieved user profile"));
        }

        private async Task<IActionResult> HandleGetTransactionsByTenantAsync<TResponse>(int? userId)
        {
            var data = await _dbContext.Transactions
                .Include(t => t.Details).ThenInclude(d => d.Category)
                .Where(t => t.Tenant!.UserId == (userId ?? 0))
                .ToListAsync();

            return new OkObjectResult(new ResponData<List<TResponse>>(true, _mapper.Map<List<TResponse>>(data), "Berhasil mengambil transaksi tenant"));
        }
        
        private async Task<IActionResult> HandleGetTransactionByIdAsync<TResponse>(int? id, int? userId)
        {
            var transaction = await _dbContext.Transactions
                .Include(t => t.Details)
                    .ThenInclude(d => d.Category)
                .FirstOrDefaultAsync(t => t.Id == (id ?? 0));

            if (transaction == null)
                return new NotFoundObjectResult(new ResponData<object>(false, "Transaksi tidak ditemukan"));

            if (transaction.Tenant!.UserId != (userId ?? 0))
                return new ObjectResult(new ResponData<object>(false, "Akses ditolak")) { StatusCode = 403 };

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(transaction), "Berhasil mengambil data transaksi"));
        }
        #endregion
    }
}