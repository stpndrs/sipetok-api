using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Repositories.Interfaces;

namespace sipetok_api.Repositories
{
    public class OperationalRepository : GenericRepository<Operational>, IOperationalRepository
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<Operational> _dbSet;

        public OperationalRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<Operational>();
        }

        public async Task<Operational> GetOperationalById(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}