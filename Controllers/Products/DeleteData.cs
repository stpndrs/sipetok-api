using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace sipetok_api.Controllers.Products
{
    public class DeleteData : IMethod // 1. Daftarkan Interface IMethod di sini
    {
        private readonly AppDbContext _dbContext;

        public DeleteData(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // =========================================================================
        // 1. INTERFACE: KHUSUS TERIMA DATA (GET) -> Di-throw karena bukan tugas Delete
        // =========================================================================
        public Task<IActionResult> ActionAsync<TEntity, TResponse>(
            string subAction, int? id = null, int? userId = null) where TEntity : class
        {
            throw new NotImplementedException("Operasi TERIMA data (GET) tidak didukung di DeleteData.");
        }

        // =========================================================================
        // 2. INTERFACE: KHUSUS HAPUS/KIRIM DATA (Menggunakan TResponse sebagai object)
        // =========================================================================
        public async Task<IActionResult> ActionAsync<TEntity, TResponse>(
            string subAction, object data, string httpMethod, int? id = null, int? userId = null) where TEntity : class
        {
            try
            {
                string entityName = typeof(TEntity).Name;
                string action = subAction?.ToLower()?.Trim() ?? string.Empty;

                // Memproses routing utama penghapusan data milikmu
                return action switch
                {
                    // --- SOFT DELETE HANDLERS ---
                    "delete_egg" => await HandleSoftDeleteEgg(id),
                    "delete_op" => await HandleSoftDeleteOp(id, userId),
                    "delete_tenant" => await HandleSoftDeleteTenant(id),

                    // --- HARD DELETE HANDLERS ---
                    "delete_category" when typeof(TEntity) == typeof(EggCategory) => await HandleHardDeleteCategory(id, userId),

                    // --- GENERIC FALLBACK ---
                    _ => await HandleGenericHardDelete<TEntity>(id, entityName)
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ResponData<object>(false, ex.Message)) { StatusCode = 500 };
            }
        }

        #region PRIVATE HANDLERS
        private async Task<IActionResult> HandleSoftDeleteEgg(int? id)
        {
            var egg = await _dbContext.Eggs.FirstOrDefaultAsync(e => e.Id == (id ?? 0));
            if (egg is null) return new NotFoundObjectResult(new ResponData<object>(false, "Egg data not found"));

            egg.SoftDelete();
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Successfully deleted egg data"));
        }

        private async Task<IActionResult> HandleSoftDeleteOp(int? id, int? userId)
        {
            var op = await _dbContext.Operationals.Include(o => o.Tenant)
                .FirstOrDefaultAsync(o => o.Id == (id ?? 0));

            if (op is null || op.Tenant!.UserId != (userId ?? 0))
                return new NotFoundObjectResult(new ResponData<object>(false, "Operational data not found"));

            op.SoftDelete();
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Successfully deleted operational data"));
        }

        private async Task<IActionResult> HandleSoftDeleteTenant(int? id)
        {
            var tenant = await _dbContext.Tenants.FindAsync(id ?? 0);
            if (tenant is null) return new NotFoundObjectResult(new ResponData<object>(false, "Tenant not found"));

            if (tenant.UserId != 0)
            {
                var user = await _dbContext.Users.FindAsync(tenant.UserId);
                if (user != null) { user.SoftDelete(); _dbContext.Users.Update(user); }
            }

            tenant.SoftDelete();
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Successfully deleted tenant data"));
        }

        private async Task<IActionResult> HandleHardDeleteCategory(int? id, int? userId)
        {
            var cat = await _dbContext.EggCategories.FindAsync(id ?? 0);
            if (cat is null) return new NotFoundObjectResult(new ResponData<object>(false, "Category not found"));

            var tenantId = await _dbContext.Tenants.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();
            if (cat.TenantId != tenantId) return new ObjectResult(new ResponData<object>(false, "Akses ditolak")) { StatusCode = 403 };

            _dbContext.EggCategories.Remove(cat);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Category deleted"));
        }

        private async Task<IActionResult> HandleGenericHardDelete<TEntity>(int? id, string name) where TEntity : class
        {
            var record = await _dbContext.Set<TEntity>().FindAsync(id ?? 0);
            if (record is null) return new NotFoundObjectResult(new ResponData<object>(false, $"{name} not found"));

            _dbContext.Set<TEntity>().Remove(record);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, $"{name} deleted"));
        }
        #endregion
    }
}