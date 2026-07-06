using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Repositories.Interfaces;

namespace sipetok_api.Repositories
{
    public class EggInventoryRepository : GenericRepository<EggInventory>, IEggInventoryRepository
    {
        public EggInventoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}