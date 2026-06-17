using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Repositories.Interfaces
{
    public interface IEggCategoryRepository : IGenericRepository<EggCategory>
    {
        Task<IEnumerable<EggCategory>> GetByTenantUserIdAsync(int userId);

        Task<EggCategory?> GetByIdWithTenantAsync(int id);
    }
}