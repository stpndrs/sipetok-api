using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Response;
using sipetok_api.helper;
using sipetok_api.Models;
using sipetok_api.Repositories;
using sipetok_api.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sipetok_api.Controllers.Products
{
    public class StevanSaveData : IStevanMethod
    {
        private readonly IConfiguration? appConfig;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly PaymentService? _paymentService;
        private readonly OrderService? _orderService;

        public StevanSaveData(AppDbContext dbContext, IConfiguration? config, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            appConfig = config;
        }

        public StevanSaveData(AppDbContext dbContext, IConfiguration? config, IMapper mapper, PaymentService paymentService, OrderService orderService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            appConfig = config;
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public Task<IActionResult> ActionAsync<TModel, TResponse>(
            TModel model, TResponse response, int? id = null, int? userId = null, string[] searchQuery = null, string[]? includes = null) where TModel : class
        {
            throw new NotImplementedException("Operasi TERIMA data (GET) tidak didukung di StevanSaveData. Gunakan StevanGetData.");
        }

        public async Task<IActionResult> ActionAsync<TModel, TResponse, TRequest>(
            TModel model, TResponse response, TRequest request, string httpMethod, int? id = null, int? userId = null, string[] searchQuery = null) where TModel : class
        {
            try
            {
                var repository = new GenericRepository<TModel>(_dbContext);
                string method = httpMethod.ToUpper();

                if (method == "POST")
                {
                    var entity = _mapper.Map<TModel>(request);

                    repository.Add(entity);
                    repository.SaveChanges();

                    if (response is AuthResponseDto authResponse)
                    {
                        string token = AuthHelper.CreateToken(entity as User, appConfig);

                        var responseAuth = _mapper.Map<TResponse>(new AuthResponseDto(token != "false" ? token : null));

                        return new OkObjectResult(new ResponData<TResponse>(true, responseAuth, "Successfully added data"));
                    }

                    var detailsProperty = entity.GetType().GetProperty("Details");
                    if (detailsProperty != null && request != null)
                    {
                        var requestDetailsProperty = request.GetType().GetProperty("Details");
                        if (requestDetailsProperty != null)
                        {
                            var detailsValue = requestDetailsProperty.GetValue(request);
                            if (detailsValue != null)
                            {
                                detailsProperty.SetValue(entity, detailsValue);

                                _dbContext.SaveChanges();
                            }
                        }
                    }

                    if (entity is EggInventory eggInventory)
                    {
                        var category = _dbContext.EggCategories.Find(eggInventory.CategoryId);
                        if (category != null)
                        {
                            category.TotalEgg += eggInventory.Stock;
                            _dbContext.SaveChanges();
                        }
                    }

                    if (entity is Transaction) return new OkObjectResult(entity);

                    var responseData = _mapper.Map<TResponse>(entity);
                    return new OkObjectResult(new ResponData<TResponse>(true, responseData, "Successfully added data"));
                }

                if (method == "PUT")
                {
                    var entity = _mapper.Map<TModel>(request);
                    int targetId = id ?? userId ?? 0;

                    if (targetId == 0)
                    {
                        return new BadRequestObjectResult(new ResponData<object?>(false, "Target ID tidak valid untuk operasi update."));
                    }

                    var existingEntity = repository.GetById(targetId);
                    if (existingEntity is null)
                    {
                        return new NotFoundObjectResult(new ResponData<object?>(false, $"Data dengan id {targetId} tidak ditemukan"));
                    }

                    if (entity is EggInventory eggInventory)
                    {
                        var category = _dbContext.EggCategories.Find(eggInventory.CategoryId);
                        var eggInventoryDb = _dbContext.EggInventories.Find(id);
                        if (category != null && eggInventoryDb != null)
                        {
                            double stockDifference = eggInventory.Stock - eggInventoryDb.Stock;
                            category.TotalEgg += stockDifference;

                            if (category.TotalEgg < 0)
                            {
                                return new BadRequestObjectResult(new ResponData<object?>(false, "Data yang dihasilkan mengakibatkan minus"));
                            }
                        }
                    }

                    _mapper.Map(request, existingEntity);

                    if (existingEntity is BaseEntity baseEntity)
                    {
                        repository.Update(existingEntity);
                        repository.SaveChanges();

                        var responseData = _mapper.Map<TResponse>(existingEntity);
                        return new OkObjectResult(new ResponData<TResponse>(true, responseData, $"Successfully updated data with id {targetId}"));
                    }
                }

                if (method == "DELETE")
                {
                    int targetId = id ?? userId ?? 0;

                    if (targetId == 0)
                    {
                        return new BadRequestObjectResult(new ResponData<object?>(false, "Target ID tidak valid untuk operasi delete."));
                    }

                    var existingEntity = repository.GetById(targetId);
                    if (existingEntity is null)
                    {
                        return new NotFoundObjectResult(new ResponData<object?>(false, $"Data dengan id {targetId} tidak ditemukan"));
                    }

                    if (existingEntity is BaseEntity baseEntity)
                    {
                        baseEntity.SoftDelete();

                        repository.Update(existingEntity);
                    }
                    else
                    {
                        repository.Delete(existingEntity);
                    }

                    repository.SaveChanges();

                    var responseData = _mapper.Map<TResponse>(existingEntity);
                    return new OkObjectResult(new ResponData<TResponse>(true, responseData, $"Successfully deleted data with id {targetId}"));
                }

                return new BadRequestObjectResult(new ResponData<object?>(false, $"HTTP Method {httpMethod} tidak didukung."));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && (ex.InnerException.Message.Contains("Duplicate") || ex.InnerException.Message.Contains("unique")))
                {
                    var errorDetail = new Dictionary<string, string[]>
                    {
                        { "Conflict", new[] { "Data terdeteksi duplikat (Email/Username/Kode sudah terdaftar)." } }
                    };
                    return new BadRequestObjectResult(new ResponValidation(errorDetail));
                }

                return new ObjectResult(new ResponData<object?>(false, "Terjadi kesalahan database saat menyimpan data.")) { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new ResponData<object?>(false, ex.Message));
            }
        }
    }
}