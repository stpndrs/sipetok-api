using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace sipetok_api.Controllers.Products
{
    public interface IStevanMethod
    {
        Task<IActionResult> ActionAsync<TModel, TResponse>(
            TModel model,
            TResponse response,
            int? id = null,
            int? userId = null,
            string[]? searchQuery = null,
            string[]? includes = null)
            where TModel : class;

        Task<IActionResult> ActionAsync<TModel, TResponse, TRequest>(
            TModel model,
            TResponse response,
            TRequest? request = default,
            string httpMethod = "POST",
            int? id = null,
            int? userId = null,
            string[]? searchQuery = null)
            where TModel : class;
    }
}