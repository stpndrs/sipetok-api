using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Repositories.Interfaces;

namespace sipetok_api.Repositories
{
    public class OperationalRepository : GenericRepository<Operational>, IOperationalRepository
    {
        public OperationalRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Operational> GetOperationalById(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}