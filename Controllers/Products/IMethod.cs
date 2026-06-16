using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace sipetok_api.Controllers.Products
{
    public interface IMethod
    {
        // 1. KHUSUS TERIMA DATA (GET / QUERY)
        // Tidak butuh parameter body 'data' ataupun 'httpMethod'
        Task<IActionResult> ActionAsync<TEntity, TResponse>(
            string subAction,
            int? id = null,
            int? userId = null)
            where TEntity : class;

        // 2. KHUSUS KIRIM DATA (POST / PUT / SAVE / COMMAND)
        // Ditambahkan parameter 'data' bertipe object umum agar bisa menerima DTO apa saja
        Task<IActionResult> ActionAsync<TEntity, TResponse>(
            string subAction,
            object data,
            string httpMethod,
            int? id = null,
            int? userId = null)
            where TEntity : class;
    }
}