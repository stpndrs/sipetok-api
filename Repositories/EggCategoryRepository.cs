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
    }
}