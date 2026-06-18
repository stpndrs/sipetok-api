using sipetok_api.Data;
using sipetok_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace sipetok_api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<IEnumerable<T>> GetWithFiltersAsync(string[]? searchQuery = null, string[]? includes = null)
        {
            IQueryable<T> query = _dbSet;

            // 1. Handle Include Relasi secara dinamis
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // 2. Handle Search Query dinamis via Reflection (Format: "NamaProperti:Nilai")
            if (searchQuery != null && searchQuery.Length > 0)
            {
                foreach (var search in searchQuery)
                {
                    if (string.IsNullOrWhiteSpace(search) || !search.Contains(":")) continue;

                    var parts = search.Split(':', 2);
                    string propertyName = parts[0].Trim();
                    string keyword = parts[1].Trim();

                    PropertyInfo? propInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propInfo != null && propInfo.PropertyType == typeof(string))
                    {
                        Console.WriteLine("query" + propInfo.Name + " keyword " + keyword);
                        query = query.Where(e => EF.Functions.Like(EF.Property<string>(e, propInfo.Name), $"%{keyword}%"));
                    }
                }
            }

            return await query.ToListAsync();
        }
    }
}