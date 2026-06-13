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
    // Mencegah aplikasi jalan tanpa kunci rahasia
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

// untuk panggil config service
builder.Services.AddApplicationServices();

// 2. OpenAPI / Swagger
builder.Services.AddOpenApi();

// 3. Database Connection

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connection = builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));
Console.WriteLine("test koneksi" + connection);
var connection2 = builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));
Console.WriteLine("test2" + connection2);
Console.WriteLine(connection2 == connection);

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
        // Menangani 403 Forbidden (Sudah Login tapi Role Tidak Sesuai)
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
            // Lewati penanganan bawaan ASP.NET agar tidak bentrok
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
    // Policy untuk mengecek apakah user aktif
    options.AddPolicy("ActiveUser", policy =>
        policy.RequireClaim("status", "active"));
});

// --- 6. BUILD APP ---
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();