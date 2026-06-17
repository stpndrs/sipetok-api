using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Data;
using sipetok_api.dto.Response;
using sipetok_api.Repositories;
using sipetok_api.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sipetok_api.Controllers.Products
{
    public class StevanGetData : IStevanMethod
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public StevanGetData(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IActionResult> ActionAsync<TModel, TResponse>(
            TModel model, TResponse response, int? id = null, int? userId = null, string[]? searchQuery = null, string[]? includes = null) where TModel : class
        {
            try
            {
                var repository = new GenericRepository<TModel>(_dbContext);

                var entities = await repository.GetWithFiltersAsync(searchQuery, includes);
                Console.WriteLine("Data Count: " + entities.Count());

                if (id != null)
                {
                    var entity = entities.FirstOrDefault(e =>
                        (int)e.GetType().GetProperty("Id")?.GetValue(e, null) == id.Value);

                    if (entity is null)
                    {
                        return new NotFoundObjectResult(new ResponData<object?>(false, $"Data dengan id {id} tidak ditemukan"));
                    }

                    var mappedResponse = _mapper.Map<TResponse>(entity);
                    return new OkObjectResult(new ResponData<TResponse>(true, mappedResponse, $"Successfully retrieved data with id {id}"));
                }
                else if (userId != null)
                {
                    var propertyInfo = typeof(TModel).GetProperty("TenantId") ?? typeof(TModel).GetProperty("UserId");

                    if (propertyInfo == null)
                    {
                        return new BadRequestObjectResult(new ResponData<object?>(false, "Model tidak memiliki properti TenantId/UserId."));
                    }

                    var filteredEntities = entities.Where(e =>
                    {
                        var value = propertyInfo.GetValue(e);
                        return value != null;
                    }).ToList();


                    var mappedResponseList = _mapper.Map<List<TResponse>>(filteredEntities);
                    return new OkObjectResult(new ResponData<List<TResponse>>(true, mappedResponseList, $"Successfully retrieved items"));
                }
                else
                {
                    var mappedResponseList = _mapper.Map<List<TResponse>>(entities.ToList());
                    return new OkObjectResult(new ResponData<List<TResponse>>(true, mappedResponseList, "Successfully retrieves data"));
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ResponData<object>(false, ex.Message)) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> ActionAsync<TModel, TResponse, TRequest>(
            TModel model, TResponse response, TRequest request, string httpMethod, int? id = null, int? userId = null, string[]? searchQuery = null) where TModel : class
        {
            throw new NotImplementedException("Operasi KIRIM data (POST/PUT) tidak didukung di StevanGetData. Gunakan SaveData.");
        }
    }
}