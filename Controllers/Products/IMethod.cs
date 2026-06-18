using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace sipetok_api.Controllers.Products
{
    public interface IMethod
    {
        Task<IActionResult> ActionAsync<TEntity, TResponse>(
            string subAction,
            int? id = null,
            int? userId = null)
            where TEntity : class;

        Task<IActionResult> ActionAsync<TEntity, TResponse>(
            string subAction,
            object data,
            string httpMethod,
            int? id = null,
            int? userId = null)
            where TEntity : class;
    }
}