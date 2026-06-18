using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Repositories.Interfaces
{
    public interface IEggInventoryRepository : IGenericRepository<EggInventory>
    {
        // Task<User?> GetUserByUsernameAsync(string username);
    }
}