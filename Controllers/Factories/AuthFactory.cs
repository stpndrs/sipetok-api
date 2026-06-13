using AutoMapper;
using Microsoft.Extensions.Configuration;
using sipetok_api.Data;
using sipetok_api.Controllers.Products; // Penting: Import agar bisa pakai SaveData
using System;

namespace sipetok_api.Controllers.Factories
{
    public class AuthFactory
    {
        // Deklarasi field yang bersih dan tidak ada duplikasi
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        // Constructor utama (pasti menerima 3 argumen ini)
        public AuthFactory(AppDbContext dbContext, IConfiguration config, IMapper mapper)
        {
            _dbContext = dbContext;
            _config = config;
            _mapper = mapper;
        }

        // Method pabrik untuk menghasilkan objek "Pekerja" (SaveData)
        public object CreateMethod(string actionType)
        {
            // Meratakan string agar tidak case-sensitive
            string action = actionType?.ToLower()?.Trim() ?? string.Empty;

            switch (action)
            {
                case "login":
                case "register":
                case "save":
                    // Mengembalikan instance SaveData dengan parameter yang diperlukan untuk Auth
                    return new SaveData(_dbContext, _mapper, config: _config);

                default:
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di AuthFactory.");
            }
        }
    }
}