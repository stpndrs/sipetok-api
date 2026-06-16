using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sipetok_api.Controllers.Products
{
    public class StevanSaveData : IStevanMethod
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public StevanSaveData(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // =========================================================================
        // 1. KHUSUS TERIMA DATA (Ditangani oleh StevanGetData.cs)
        // =========================================================================
        public Task<IActionResult> ActionAsync<TModel, TResponse>(
            TModel model, TResponse response, int? id = null, int? userId = null, string[]? includes = null) where TModel : class
        {
            throw new NotImplementedException("Operasi TERIMA data (GET) tidak didukung di StevanSaveData. Gunakan StevanGetData.");
        }

        // =========================================================================
        // 2. KHUSUS KIRIM DATA (POST / PUT / SAVE / COMMAND) -> Logika asli kelas ini
        // =========================================================================
        public async Task<IActionResult> ActionAsync<TModel, TResponse, TRequest>(
            TModel model, TResponse response, TRequest request, string httpMethod, int? id = null, int? userId = null) where TModel : class
        {
            try
            {
                var repository = new GenericRepository<TModel>(_dbContext);
                string method = httpMethod.ToUpper();

                if (method == "POST")
                {
                    var entity = _mapper.Map<TModel>(request);

                    // tambahkan logika logika khusus...
                    if (entity is User userEntity)
                    {
                        if (string.IsNullOrWhiteSpace(userEntity.Password))
                        {
                            return new BadRequestObjectResult(new ResponData<object?>(false, "Password is required"));
                        }
                        userEntity.Password = Bcrypt.HashPassword(userEntity.Password);
                    }

                    repository.Add(entity);
                    repository.SaveChanges();

                    var responseData = _mapper.Map<TResponse>(entity);
                    return new OkObjectResult(new ResponData<TResponse>(true, responseData, "Successfully added data"));
                }

                if (method == "PUT")
                {
                    int targetId = id ?? userId ?? 0;

                    if (targetId == 0)
                    {
                        return new BadRequestObjectResult(new ResponData<object?>(false, "Target ID tidak valid untuk operasi update."));
                    }

                    // Cari data lama di database
                    var existingEntity = repository.GetById(targetId);
                    if (existingEntity is null)
                    {
                        return new NotFoundObjectResult(new ResponData<object?>(false, $"Data dengan id {targetId} tidak ditemukan"));
                    }

                    _mapper.Map(request, existingEntity);

                    // tambahkan logika khusus...
                    if (existingEntity is User userToUpdate)
                    {
                        dynamic dtoRequest = request!;
                        try
                        {
                            if (!string.IsNullOrWhiteSpace((string)dtoRequest.Password))
                            {
                                userToUpdate.Password = Bcrypt.HashPassword((string)dtoRequest.Password);
                            }
                        }
                        catch { /* Abaikan jika TRequest modul lain tidak punya properti Password */ }

                        userToUpdate.UpdateTimestamps();
                    }

                    repository.Update(existingEntity);
                    repository.SaveChanges();

                    var responseData = _mapper.Map<TResponse>(existingEntity);
                    return new OkObjectResult(new ResponData<TResponse>(true, responseData, $"Successfully updated data with id {targetId}"));
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
                // tambahkan logika khusus
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