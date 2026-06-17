using System; // Dibutuhkan untuk ArgumentException
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
        {
            switch (actionType.ToLower())
            {
                case "get":
                case "read":
                    return new StevanGetData(_dbContext, _mapper);

                case "save":
                case "write":
                    return new StevanSaveData(_dbContext, _config, _mapper);

                case "delete":
                case "remove":

                    return new DeleteData(_dbContext) as IStevanMethod ?? throw new InvalidCastException("DeleteData tidak mengimplementasikan IStevanMethod");

                default:
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di EggCategoryFactory.");
            }
        }
    }
}