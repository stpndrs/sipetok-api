namespace sipetok_api.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
        Task<IEnumerable<T>> GetWithFiltersAsync(string[]? searchQuery = null, string[]? includes = null);
    }
}