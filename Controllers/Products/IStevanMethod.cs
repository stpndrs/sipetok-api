using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace sipetok_api.Controllers.Products
{
    public interface IStevanMethod
    {
        // 1. KHUSUS TERIMA DATA (GET / QUERY)
        Task<IActionResult> ActionAsync<TModel, TResponse>(
            TModel model,
            TResponse response,
            int? id = null,
            int? userId = null,
            string[]? includes = null)
            where TModel : class;

        // 2. KHUSUS KIRIM DATA (POST / PUT / SAVE / COMMAND)
        Task<IActionResult> ActionAsync<TModel, TResponse, TRequest>(
            TModel model,
            TResponse response,
            TRequest? request = default,
            string httpMethod = "POST",
            int? id = null,
            int? userId = null)
            where TModel : class;
    }
}