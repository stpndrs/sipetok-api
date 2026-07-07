using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Repositories.Interfaces;

namespace sipetok_api.Repositories
{
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Tenant> GetTenantByUserId(int userId)
        {
            return await _context.Tenants.FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}