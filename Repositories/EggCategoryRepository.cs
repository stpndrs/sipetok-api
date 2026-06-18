using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Repositories.Interfaces;

namespace sipetok_api.Repositories
{
    public class EggCategoryRepository : GenericRepository<EggCategory>, IEggCategoryRepository
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<Tenant> _dbSet;

        public EggCategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<Tenant>();
        }

        public async Task<EggCategory> GetEggCategoryByTenantId(int tenantId)
        {
            return await _context.EggCategories.FirstOrDefaultAsync(a => a.TenantId == tenantId);
        }
    }
}