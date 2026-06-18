using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Repositories.Interfaces
{
    public interface ITenantRepository : IGenericRepository<Tenant>
    {
        Task<Tenant> GetTenantByUserId(int userId);
    }
}