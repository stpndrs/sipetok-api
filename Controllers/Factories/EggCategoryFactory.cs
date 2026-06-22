using System; 
using AutoMapper;
using Microsoft.Extensions.Configuration;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;

namespace sipetok_api.Controllers.Factories
{
    public class EggCategoryFactory : StevanModuleFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config; 

        public EggCategoryFactory(AppDbContext dbContext, IConfiguration config, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _config = config;
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