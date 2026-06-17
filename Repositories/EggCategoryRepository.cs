using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Repositories.Interfaces;

namespace sipetok_api.Repositories
{
    public class EggCategoryRepository : GenericRepository<EggCategory>, IEggCategoryRepository
    {
        public EggCategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EggCategory>> GetByTenantUserIdAsync(int userId)
        {
            return await _context.EggCategories
                .Include(c => c.Tenant)
                .Where(c => c.Tenant != null && c.Tenant.UserId == userId)
                .ToListAsync();
        }
        
        public async Task<EggCategory?> GetByIdWithTenantAsync(int id)
        {
            return await _context.EggCategories
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}