using AutoMapper;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public class TenantFactory : StevanModuleFactory
    {
        private readonly IConfiguration appConfig;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public TenantFactory(AppDbContext dbContext, IConfiguration config, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            appConfig = config;
        }

        public IStevanMethod CreateMethod(string actionType)
        {
            string action = actionType?.ToLower()?.Trim() ?? string.Empty;

            return action switch
            {
                "get" or "read" => new StevanGetData(_dbContext, _mapper),
                "save" or "write" => new StevanSaveData(_dbContext, appConfig, _mapper),
                // "delete" or "remove" => new DeleteData(_dbContext),
                // _ => throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di UserFactory.")
            };
        }
    }
}