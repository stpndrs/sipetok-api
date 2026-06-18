using AutoMapper;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;
using System;

namespace sipetok_api.Controllers.Factories
{
    public class EggInventoryFactory : StevanModuleFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public EggInventoryFactory(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IStevanMethod CreateMethod(string actionType)
        {
            string action = actionType?.ToLower()?.Trim() ?? string.Empty;

            return action switch
            {
                "get" or "read" => new StevanGetData(_dbContext, _mapper),
                "save" or "write" => new StevanSaveData(_dbContext, null, _mapper),
            };
        }
    }
}