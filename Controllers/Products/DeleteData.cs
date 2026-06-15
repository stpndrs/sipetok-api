using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using System;
using System.Threading.Tasks;

namespace sipetok_api.Controllers.Products
{
    public class DeleteData
    {
        private readonly AppDbContext _dbContext;

        public DeleteData(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> ActionAsync<TEntity>(string subAction, int? id = null, int? userId = null) where TEntity : class
        {
            try
            {
                string entityName = typeof(TEntity).Name;
                string action = subAction?.ToLower()?.Trim() ?? string.Empty;

                return action switch
                {
                    // --- SOFT DELETE HANDLERS ---
                    "delete_egg" => await HandleSoftDeleteEgg(id, userId),
                    "delete_operational" => await HandleSoftDeleteOp(id, userId),
                    "delete_tenant" => await HandleSoftDeleteTenant(id),
                    "delete_user" => await HandleSoftDeleteUser(id),
                    "delete_category" => await HandleSoftDeleteCategory(id, userId),

                    // --- HARD DELETE HANDLERS ---
                    // "delete_category" when typeof(TEntity) == typeof(EggCategory) => await HandleHardDeleteCategory(id, userId),

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
        private async Task<IActionResult> HandleSoftDeleteEgg(int? id, int? userId)
        {
            try
            {
                var egg = await _dbContext.Eggs
                    .Include(e => e.Category)
                        .ThenInclude(cat => cat!.Tenant)
                    .FirstOrDefaultAsync(e => e.Id == (id ?? 0));

                if (egg is null)
                    return new NotFoundObjectResult(new ResponData<object>(false, "Egg tidak ditemukan"));

                if (egg.Category is null || egg.Category.Tenant is null)
                {
                    return new BadRequestObjectResult(new ResponData<object>(false, "Data kategori atau tenant terkait tidak ditemukan"));
                }

                if (egg.Category.Tenant.UserId != (userId ?? 0))
                {
                    return new ObjectResult(new ResponData<object>(false, "Akses ditolak, ini bukan data Anda")) { StatusCode = 403 };
                }

                egg.SoftDelete();
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult(new ResponData<object>(true, "Sukses menghapus data telur"));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ResponData<object>(false, ex.Message)) { StatusCode = 500 };
            }
        }

        private async Task<IActionResult> HandleSoftDeleteOp(int? id, int? userId)
        {
            var op = await _dbContext.Operationals.Include(o => o.Tenant)
                .FirstOrDefaultAsync(o => o.Id == (id ?? 0));

            if (op is null)
                return new NotFoundObjectResult(new ResponData<object>(false, "Data operasional tidak ditemukan"));

            if (op.Tenant == null || op.Tenant.UserId != userId)
                return new ObjectResult(new ResponData<object>(false, "Akses ditolak, ini bukan data Anda")) { StatusCode = 403 };

            op.SoftDelete();
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Sukses menghapus data operasional"));
        }

        private async Task<IActionResult> HandleSoftDeleteTenant(int? id)
        {
            var tenant = await _dbContext.Tenants
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == (id ?? 0));

            if (tenant is null) return new NotFoundObjectResult(new ResponData<object>(false, "Tenant tidak ditemukan"));

            if (tenant.UserId != 0)
            {
                if (tenant.User != null) tenant.User.SoftDelete();
            }

            tenant.SoftDelete();
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Sukses menghapus data tenant"));
        }

        // private async Task<IActionResult> HandleHardDeleteCategory(int? id, int? userId)
        // {
        //     var cat = await _dbContext.EggCategories.FindAsync(id ?? 0);
        //     if (cat is null) return new NotFoundObjectResult(new ResponData<object>(false, "Category not found"));

        //     var tenantId = await _dbContext.Tenants.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefaultAsync();
        //     if (cat.TenantId != tenantId) return new ObjectResult(new ResponData<object>(false, "Akses ditolak")) { StatusCode = 403 };

        //     _dbContext.EggCategories.Remove(cat);
        //     await _dbContext.SaveChangesAsync();
        //     return new OkObjectResult(new ResponData<object>(true, "Category deleted"));
        // }

        private async Task<IActionResult> HandleSoftDeleteUser(int? id)
        {
            var user = await _dbContext.Users.FindAsync(id ?? 0);

            if (user is null) return new NotFoundObjectResult(new ResponData<object>(false, "User tidak ditemukan"));

            user.SoftDelete();
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Sukses menghapus data user"));
        }

        private async Task<IActionResult> HandleSoftDeleteCategory(int? id, int? userId)
        {
            var category = await _dbContext.EggCategories
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == (id ?? 0));

            if (category is null)
                return new NotFoundObjectResult(new ResponData<object>(false, "Category tidak ditemukan"));

            if (category.Tenant == null || category.Tenant.UserId != (userId ?? 0))
                return new ObjectResult(new ResponData<object>(false, "Akses ditolak, ini bukan data Anda")) { StatusCode = 403 };

            category.SoftDelete();
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, "Sukses menghapus data kategori"));
        }

        private async Task<IActionResult> HandleGenericHardDelete<TEntity>(int? id, string name) where TEntity : class
        {
            var record = await _dbContext.Set<TEntity>().FindAsync(id ?? 0);
            if (record is null) return new NotFoundObjectResult(new ResponData<object>(false, $"{name} tidak ditemukan"));

            _dbContext.Set<TEntity>().Remove(record);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<object>(true, $"{name} dihapus"));
        }
        #endregion
    }
}