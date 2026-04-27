using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions; 
using sipetok_api;
using sipetok_api.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Gabungkan semua konfigurasi Controller di sini
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Memaksa Enum tampil sebagai Teks (String) di JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Tips: Abaikan siklus jika nanti ada relasi timbal balik (Optional)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 2. OpenAPI / Swagger
builder.Services.AddOpenApi();

// 3. Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString)); 

// 4. AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

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