using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Services;
using sipetok_api.Utils;
using sipetok_api.Services; // Pastikan namespace AccountRoleTableDriven ada di sini
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace sipetok_api.Controllers.Products
{
    public class SaveData
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration? _config; // Ditambahkan untuk JWT Auth
        private readonly PaymentService? _paymentService;
        private readonly OrderService? _orderService;

        // Constructor dibuat fleksibel dengan parameter opsional
        public SaveData(
            AppDbContext dbContext,
            IMapper mapper,
            PaymentService? paymentService = null,
            OrderService? orderService = null,
            IConfiguration? config = null) // Tambahan parameter konfigurasi JWT
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _paymentService = paymentService;
            _orderService = orderService;
            _config = config;
        }

        public async Task<IActionResult> ActionAsync<TEntity, TRequest, TResponse>(
            string subAction, TRequest data, string httpMethod, int? id = null, int? userId = null) where TEntity : class
        {
            try
            {
                string entityName = typeof(TEntity).Name;
                string action = subAction.ToLower().Trim();
                string method = httpMethod.ToUpper().Trim();

                // Router Utama menggunakan Switch Expression (Pure API Logic)
                return method switch
                {
                    "POST" => action switch
                    {
                        // --- AUTH ACTIONS ---
                        "register" => await HandleRegisterAsync<TResponse>(data),
                        "login" => await HandleLoginAsync<TResponse>(data),

                        // --- TRANSACTION ENGINE ---
                        "tx_store" => await HandleTxStoreAsync<TResponse>(data),
                        "tx_pay" => await HandleTxPayAsync<TResponse>(id, data),
                        "tx_cancel" => await HandleTxCancelAsync<TResponse>(id),
                        "tx_complete" => await HandleTxCompleteAsync<TResponse>(id),

                        // --- CRUD POST ---
                        "add_category" when typeof(TEntity) == typeof(EggCategory) => await HandleAddCategoryAsync<TEntity, TResponse>(data, userId),
                        "add_egg" => await HandleAddEggAsync<TResponse>(data, userId),
                        "add_op" => await HandleAddOperationalAsync<TResponse>(data, userId),
                        "add_tenant" => await HandleAddTenantAsync<TResponse>(data),
                        "validate_tenant" => await HandleValidateTenantAsync<TResponse>(id),
                        _ => await HandleGenericPostAsync<TEntity, TResponse>(data, entityName)
                    },
                    "PUT" => action switch
                    {
                        // --- CRUD PUT ---
                        "update_op" => await HandleUpdateOperationalAsync<TResponse>(id, data, userId),
                        "update_egg" => await HandleUpdateEggAsync<TResponse>(id, data),
                        "update_tenant" => await HandleUpdateTenantAsync<TResponse>(id, data),
                        "update_myprofile" => await HandleUpdateMyProfileAsync<TResponse>(data, userId),
                        _ => await HandleGenericPutAsync<TEntity, TResponse>(id, action, data, userId, entityName)
                    },
                    _ => throw new NotSupportedException($"HTTP Method {httpMethod} tidak didukung di SaveData Backend.")
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ResponData<object>(false, ex.Message)) { StatusCode = 500 };
            }
        }

        #region AUTH ACTION HANDLERS
        private async Task<IActionResult> HandleRegisterAsync<TResponse>(object? data)
        {
            if (data is not RegisterDto req) return InvalidDtoResponse();

            try
            {
                string passwordHash = Bcrypt.BcryptPassword(req.Password);
                var user = new User
                {
                    Username = req.Username,
                    Email = req.Email,
                    Password = passwordHash,
                    Role = 3, // Default Customer
                    IsActive = true
                };

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                string token = CreateToken(user);
                var authRespon = new AuthRespon(token);

                // Map hasil akhir sesuai dengan TResponse yang diminta Controller (AuthRespon)
                var mappedResult = _mapper.Map<TResponse>(authRespon);

                return new OkObjectResult(new ResponData<TResponse>(true, mappedResult, "Register berhasil"));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && (ex.InnerException.Message.Contains("Duplicate") || ex.InnerException.Message.Contains("unique")))
                {
                    var errorDetail = new Dictionary<string, string[]>
                    {
                        { "Account", new[] { "Email atau Username sudah terdaftar, silakan gunakan yang lain." } }
                    };
                    return new BadRequestObjectResult(new ResponValidation(errorDetail));
                }
                return new ObjectResult(new ResponData<object>(false, "Terjadi kesalahan saat menyimpan data ke database.")) { StatusCode = 500 };
            }
        }

        private async Task<IActionResult> HandleLoginAsync<TResponse>(object? data)
        {
            if (data is not LoginDto req) return InvalidDtoResponse();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == req.Username);

            if (user == null || !Bcrypt.VerifyPassword(req.Password, user.Password))
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Wrong Username or Password"));
            }

            if (!user.IsActive)
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Your account has been deactivated"));
            }

            string token = CreateToken(user);
            var authRespon = new AuthRespon(token);

            var mappedResult = _mapper.Map<TResponse>(authRespon);

            return new OkObjectResult(new ResponData<TResponse>(true, mappedResult, "Login berhasil"));
        }
        #endregion

        #region POST HANDLERS (TRANSACTION STATE ENGINE & CRUD)
        private async Task<IActionResult> HandleTxStoreAsync<TResponse>(object data)
        {
            if (data is not TransactionDto transactionDto) return InvalidDtoResponse();

            var transaction = await _paymentService!.ProcessTransaction(transactionDto);
            var completeData = await _dbContext.Transactions
                .Include(t => t.Details)
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(completeData), "Berhasil menambahkan transaksi (Orderan Masuk & Menunggu Pembayaran)"));
        }

        private async Task<IActionResult> HandleTxPayAsync<TResponse>(int? id, object data)
        {
            if (data is not PaymentDto paymentDto) return InvalidDtoResponse();

            var success = await _paymentService!.UpdateStatus(id ?? 0, PaymentTrigger.Pay, paymentDto);
            if (!success) return new BadRequestObjectResult(new ResponData<object>(false, "Gagal memproses pembayaran. Pastikan ID benar atau status saat ini valid."));

            var transaction = await _dbContext.Transactions
                .Include(t => t.Details)
                .FirstOrDefaultAsync(t => t.Id == (id ?? 0));

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(transaction), "Pembayaran sukses dicatat, stok telur berhasil dikurangi, dan pesanan SIAP DIAMBIL."));
        }

        private async Task<IActionResult> HandleTxCancelAsync<TResponse>(int? id)
        {
            var success = await _paymentService!.UpdateStatus(id ?? 0, PaymentTrigger.Cancel, null);
            if (!success) return new BadRequestObjectResult(new ResponData<object>(false, "Transaksi tidak ditemukan atau tidak dapat dibatalkan pada status saat ini."));

            var transaction = await _dbContext.Transactions
                .Include(t => t.Details)
                .FirstOrDefaultAsync(t => t.Id == (id ?? 0));

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(transaction), "Transaksi dan pesanan telah berhasil dibatalkan."));
        }

        private async Task<IActionResult> HandleTxCompleteAsync<TResponse>(int? id)
        {
            var transaction = await _dbContext.Transactions
                .Include(t => t.Details)
                .FirstOrDefaultAsync(t => t.Id == (id ?? 0));

            if (transaction == null) return new NotFoundObjectResult(new ResponData<object>(false, "Transaksi tidak ditemukan."));

            var isUpdated = _orderService!.UpdateOrderStatus(transaction, OrderTrigger.PickedUp);
            if (!isUpdated) return new BadRequestObjectResult(new ResponData<object>(false, "Gagal menyelesaikan pesanan. Pastikan status pesanan saat ini adalah 'ReadyForPickup' (Siap Diambil)."));

            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(transaction), "Pesanan selesai! Telur telah diambil oleh pelanggan."));
        }

        private async Task<IActionResult> HandleAddCategoryAsync<TEntity, TResponse>(object data, int? userId) where TEntity : class
        {
            var entity = _mapper.Map<TEntity>(data);
            if (entity is EggCategory eggCategory)
            {
                var tenantId = await GetTenantIdByUserIdAsync(userId);
                if (tenantId == 0) return new BadRequestObjectResult(new ResponData<object>(false, "Profil Tenant tidak ditemukan untuk user ini."));
                eggCategory.TenantId = tenantId;
            }

            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(entity), $"Successfully created new {typeof(TEntity).Name} data"));
        }

        private async Task<IActionResult> HandleAddEggAsync<TResponse>(object data, int? userId)
        {
            if (data is not EggDto eggDto) return InvalidDtoResponse();

            var tenant = await GetTenantByUserIdAsync(userId);
            var eggCategory = await _dbContext.EggCategories.FindAsync(eggDto.CategoryId);

            if (tenant is null) return new BadRequestObjectResult(new ResponData<object>(false, "Data Tenant not found"));
            if (eggCategory is null) return new BadRequestObjectResult(new ResponData<object>(false, "Data Category not found"));
            if (eggCategory.TenantId != tenant.Id) return new BadRequestObjectResult(new ResponData<object>(false, "Category does not belong to your Tenant"));

            var egg = _mapper.Map<Egg>(eggDto);
            egg.CategoryId = eggCategory.Id;

            await _dbContext.Eggs.AddAsync(egg);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(egg), "Successfully added egg data"));
        }

        private async Task<IActionResult> HandleAddOperationalAsync<TResponse>(object data, int? userId)
        {
            if (data is not OperationalDto opDto) return InvalidDtoResponse();

            var tenant = await GetTenantByUserIdAsync(userId);
            if (tenant is null) return new BadRequestObjectResult(new ResponData<object>(false, "Tenant profile not found"));

            var operational = _mapper.Map<Operational>(opDto);
            operational.TenantId = tenant.Id;

            await _dbContext.Operationals.AddAsync(operational);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(operational), "Successfully added operational data"));
        }

        private async Task<IActionResult> HandleAddTenantAsync<TResponse>(object data)
        {
            if (data is not TenantDto tenantDto) return InvalidDtoResponse();
            if (tenantDto.User == null) return new BadRequestObjectResult(new ResponData<object>(false, "User is required"));
            if (string.IsNullOrWhiteSpace(tenantDto.User.Password)) return new BadRequestObjectResult(new ResponData<object>(false, "Password is required"));

            var userEntity = _mapper.Map<User>(tenantDto.User);
            userEntity.Password = Bcrypt.BcryptPassword(userEntity.Password);
            userEntity.Role = 2;
            userEntity.IsActive = true;
            tenantDto.IsValid = false;

            var tenant = _mapper.Map<Tenant>(tenantDto);
            tenant.User = userEntity;

            await _dbContext.Tenants.AddAsync(tenant);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(tenant), "Successfully added tenant data"));
        }

        private async Task<IActionResult> HandleValidateTenantAsync<TResponse>(int? id)
        {
            var tenant = await _dbContext.Tenants.FindAsync(id ?? 0);
            if (tenant is null) return new NotFoundObjectResult(new ResponData<object>(false, $"Tenant data with id {id} not found"));

            tenant.IsValid = true;
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(tenant), $"Successfully validated tenant data with id {id}"));
        }

        private async Task<IActionResult> HandleGenericPostAsync<TEntity, TResponse>(object data, string entityName) where TEntity : class
        {
            var entity = _mapper.Map<TEntity>(data);
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(entity), $"Successfully created new {entityName} data"));
        }
        #endregion

        #region PUT HANDLERS
        private async Task<IActionResult> HandleUpdateOperationalAsync<TResponse>(int? id, object data, int? userId)
        {
            if (data is not OperationalDto editOpDto) return InvalidDtoResponse();

            var operational = await _dbContext.Operationals
                .Include(o => o.Tenant)
                .FirstOrDefaultAsync(o => o.Id == (id ?? 0));

            if (operational is null || operational.Tenant!.UserId != (userId ?? 0))
            {
                return new NotFoundObjectResult(new ResponData<object>(false, $"Operational data with id {id} not found"));
            }

            _mapper.Map(editOpDto, operational);
            operational.UpdateTimestamps();

            _dbContext.Operationals.Update(operational);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(operational), "Successfully updated operational data"));
        }

        private async Task<IActionResult> HandleUpdateEggAsync<TResponse>(int? id, object data)
        {
            if (data is not EggDto editEggDto) return InvalidDtoResponse();

            var egg = await _dbContext.Eggs.FirstOrDefaultAsync(e => e.Id == (id ?? 0));
            if (egg == null) return new NotFoundObjectResult(new ResponData<object>(false, "Data egg not found"));

            egg.ProductionDate = editEggDto.ProductionDate;
            egg.CategoryId = editEggDto.CategoryId;
            egg.Stock = editEggDto.Stock;
            egg.UpdateTimestamps();

            _dbContext.Eggs.Update(egg);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(egg), "Successfully updated egg data"));
        }

        private async Task<IActionResult> HandleUpdateTenantAsync<TResponse>(int? id, object data)
        {
            if (data is not TenantDto editTenantDto) return InvalidDtoResponse();

            var tenant = await _dbContext.Tenants.FindAsync(id ?? 0);
            if (tenant is null) return new NotFoundObjectResult(new ResponData<object>(false, $"Tenant data with id {id} not found"));

            _mapper.Map(editTenantDto, tenant);
            tenant.UpdateTimestamps();

            if (editTenantDto.User != null)
            {
                var userEntity = await _dbContext.Users.FindAsync(tenant.UserId);
                if (userEntity != null)
                {
                    _mapper.Map(editTenantDto.User, userEntity);
                    if (!string.IsNullOrWhiteSpace(editTenantDto.User.Password))
                    {
                        userEntity.Password = Bcrypt.BcryptPassword(editTenantDto.User.Password);
                    }
                    userEntity.UpdateTimestamps();
                    _dbContext.Users.Update(userEntity);
                }
            }

            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(tenant), $"Successfully updated tenant data with id {id}"));
        }

        private async Task<IActionResult> HandleUpdateMyProfileAsync<TResponse>(object data, int? userId)
        {
            if (data is not TenantDto profileDto) return InvalidDtoResponse();

            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.UserId == (userId ?? 0));
            if (tenant is null) return new NotFoundObjectResult(new ResponData<object>(false, $"Tenant data with id {userId} not found"));

            _mapper.Map(profileDto, tenant);
            tenant.UpdateTimestamps();

            if (profileDto.User != null)
            {
                var userEntity = await _dbContext.Users.FindAsync(tenant.UserId);
                if (userEntity != null)
                {
                    if (!string.IsNullOrWhiteSpace(profileDto.User.Password))
                    {
                        userEntity.Password = Bcrypt.BcryptPassword(profileDto.User.Password);
                    }
                    _mapper.Map(profileDto.User, userEntity);
                    userEntity.Role = 2;
                    userEntity.IsActive = true;
                    userEntity.UpdateTimestamps();
                    _dbContext.Users.Update(userEntity);
                }
            }

            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(tenant), $"Successfully updated tenant data with id {userId}"));
        }

        private async Task<IActionResult> HandleGenericPutAsync<TEntity, TResponse>(
            int? id, string action, object data, int? userId, string entityName) where TEntity : class
        {
            var existingRecord = await _dbContext.Set<TEntity>().FindAsync(id ?? 0);
            if (existingRecord is null) return new NotFoundObjectResult(new ResponData<object>(false, $"{entityName} data with id {id} not found"));

            if (action == "update_category" && existingRecord is EggCategory eggCategory)
            {
                var tenantId = await GetTenantIdByUserIdAsync(userId);
                if (eggCategory.TenantId != tenantId)
                {
                    return new ObjectResult(new ResponData<object>(false, "Anda tidak memiliki akses untuk mengubah kategori ini.")) { StatusCode = 403 };
                }
            }

            _mapper.Map(data, existingRecord);
            _dbContext.Set<TEntity>().Update(existingRecord);
            await _dbContext.SaveChangesAsync();

            return new OkObjectResult(new ResponData<TResponse>(true, _mapper.Map<TResponse>(existingRecord), $"Successfully updated {entityName} data with id {id}"));
        }
        #endregion

        #region TOKEN GENERATOR UTILITY
        private string CreateToken(User user)
        {
            if (_config == null) throw new InvalidOperationException("IConfiguration belum dikonfigurasi di SaveData.");

            var roleService = new AccountRoleTableDriven();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, roleService.GetRoleName(user.Role)),
                new Claim("userId", user.Id.ToString()),
            };

            var jwtSection = _config.GetSection("configProperties:JWT");
            var issuer = jwtSection["JWT_ISSUER"];
            var audience = jwtSection["JWT_AUDIENCE"];
            var keyString = jwtSection["JWT_KEY"] ?? throw new InvalidOperationException("JWT Secret Key tidak ditemukan di appsettings.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region HELPER UTILITIES
        private async Task<int> GetTenantIdByUserIdAsync(int? userId)
        {
            return await _dbContext.Tenants
                .Where(t => t.UserId == (userId ?? 0))
                .Select(t => t.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<Tenant?> GetTenantByUserIdAsync(int? userId)
        {
            return await _dbContext.Tenants.FirstOrDefaultAsync(c => c.UserId == (userId ?? 0));
        }

        private BadRequestObjectResult InvalidDtoResponse()
        {
            return new BadRequestObjectResult(new ResponData<object>(false, "Format payload Request DTO tidak valid atau tidak cocok."));
        }
        #endregion
    }
}