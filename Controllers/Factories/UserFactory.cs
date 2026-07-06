using AutoMapper;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;
using System;

namespace sipetok_api.Controllers.Factories
{
    public class UserFactory : IStevanModuleFactory
    {
        private readonly IConfiguration appConfig;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserFactory(AppDbContext dbContext, IConfiguration config, IMapper mapper)
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
                _ => throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di UserFactory."),
            };
        }
    }
}