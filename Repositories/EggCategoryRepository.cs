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

        public async Task<EggCategory> GetEggCategoryByTenantId(int tenantId)
        {
            return await _context.EggCategories.FirstOrDefaultAsync(a => a.TenantId == tenantId);
        }
    }
}