using sipetok_api.Models;

namespace sipetok_api.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Jika nanti butuh query spesifik untuk User (misal: GetByEmail), tulis di sini.
    }
}