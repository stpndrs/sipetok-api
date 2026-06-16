using AutoMapper;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;
using System;

namespace sipetok_api.Controllers.Factories
{
    public class UserFactory : StevanModuleFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserFactory(AppDbContext dbContext, IMapper mapper)
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
                "save" or "write" => new StevanSaveData(_dbContext, _mapper),
                // "delete" or "remove" => new DeleteData(_dbContext),
                // _ => throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di UserFactory.")
            };
        }
    }
}