using AutoMapper;
using Microsoft.Extensions.Configuration;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;
using System;

namespace sipetok_api.Controllers.Factories
{
    public class AuthFactory : IStevanModuleFactory
    {
        private readonly IConfiguration appConfig;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuthFactory(AppDbContext dbContext, IConfiguration config, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            appConfig = config;
        }

        public IStevanMethod CreateMethod(string actionType)
        {
            string action = actionType?.ToLower()?.Trim() ?? string.Empty;

            switch (action)
            {
                case "login":
                case "register":
                case "save":
                    return new StevanSaveData(_dbContext, appConfig, _mapper);

                case "get":
                    return new StevanGetData(_dbContext, _mapper);

                default:
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di AuthFactory.");
            }
        }
    }
}