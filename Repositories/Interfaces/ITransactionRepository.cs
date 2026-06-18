using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Repositories.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        // Task<Transaction?> GetTransctionDetailByTransactionId(int transactionId);
    }
}