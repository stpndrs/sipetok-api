using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using sipetok_api;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using sipetok_api.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 0. LOAD JWT CONFIGURATION ---
var jwtSection = builder.Configuration.GetSection("configProperties:JWT");
var keyString = jwtSection["JWT_KEY"];

if (string.IsNullOrEmpty(keyString))
{
    throw new Exception("JWT Key is missing in appsettings.json! Check configProperties:JWT:JWT_KEY");
}

var key = Encoding.UTF8.GetBytes(keyString);

// --- 1. CONTROLLER & JSON CONFIGURATION ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var validationErrors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            ResponValidation response = new ResponValidation(validationErrors);

            return new BadRequestObjectResult(response);
        };
    });

// Panggil konfigurasi service bawaan aplikasi
builder.Services.AddApplicationServices();

// PERBAIKAN: Registrasi TenantFactory untuk mengatasi error 'Unable to resolve service' sebelumnya
builder.Services.AddScoped<sipetok_api.Controllers.Factories.TenantFactory>();
builder.Services.AddScoped<sipetok_api.Controllers.Factories.EggInventoryFactory>();
builder.Services.AddScoped<sipetok_api.Controllers.Factories.EggCategoryFactory>();
builder.Services.AddScoped<sipetok_api.Controllers.Factories.TransactionFactory>();
builder.Services.AddScoped<sipetok_api.Controllers.Factories.OperationalFactory>();
builder.Services.AddScoped<sipetok_api.Controllers.Factories.UserFactory>();

// 2. OpenAPI / Swagger
builder.Services.AddOpenApi();

// 3. Database Connection (Cukup bersihkan dan panggil 1 kali saja)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

// 4. AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// --- 5. AUTHENTICATION & AUTHORIZATION ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["JWT_ISSUER"],
        ValidAudience = jwtSection["JWT_AUDIENCE"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnForbidden = async context =>
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var respon = new ResponData<object?>(false, "Forbidden: You do not have permission to access this resource.");

            var jsonRespon = JsonSerializer.Serialize(respon, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonRespon);
        },

        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var respon = new ResponData<object?>(false, "Unauthorized: Access is denied due to invalid or missing credentials.");

            var jsonRespon = JsonSerializer.Serialize(respon, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonRespon);
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ActiveUser", policy =>
        policy.RequireClaim("status", "active"));
});

// --- 6. BUILD APP ---
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

// PERBAIKAN UTAMA: Wajib jalankan UseAuthentication() SEBELUM UseAuthorization()
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();