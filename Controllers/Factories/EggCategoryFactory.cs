<<<<<<< HEAD
﻿using AutoMapper;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public class EggCategoryFactory : ModuleFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        // Constructor untuk mengambil DbContext & Mapper dari DI Container .NET
        public EggCategoryFactory(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // Method pabrik untuk membuat objek pekerja berdasarkan actionType
        public IMethod CreateMethod(string actionType)
=======
﻿using System; // Dibutuhkan untuk ArgumentException
using AutoMapper;
using Microsoft.Extensions.Configuration; // Dibutuhkan untuk IConfiguration
using sipetok_api.Controllers.Products;
using sipetok_api.Data;

namespace sipetok_api.Controllers.Factories
{
    public class EggCategoryFactory : StevanModuleFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config; // 1. Deklarasikan field configuration di sini

        // 2. Hapus koma gantung di ujung parameter mapper
        public EggCategoryFactory(AppDbContext dbContext, IConfiguration config, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _config = config; // 3. Set nilainya ke field yang benar
        }

        public IStevanMethod CreateMethod(string actionType)
>>>>>>> 66185cb9672652d715a413bd97d21b5b6f10fbf7
        {
            switch (actionType.ToLower())
            {
                case "get":
                case "read":
<<<<<<< HEAD
                    return new GetData(_dbContext, _mapper);

                case "save":
                case "write":
                    return new SaveData(_dbContext, _mapper);

                case "delete":
                case "remove":
                    return new DeleteData(_dbContext);
=======
                    return new StevanGetData(_dbContext, _mapper);

                case "save":
                case "write":
                    return new StevanSaveData(_dbContext, _config, _mapper);
>>>>>>> 66185cb9672652d715a413bd97d21b5b6f10fbf7

                default:
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di EggCategoryFactory.");
            }
        }
    }
}