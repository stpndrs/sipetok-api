using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Repositories.Interfaces;

namespace sipetok_api.Repositories
{
    public class TransactionRepository : GenericRepository<User>, IUserRepository
    {
        public TransactionRepository(AppDbContext context) : base(context)
        {
            // Semua context dioper ke GenericRepository
        }
    }
}