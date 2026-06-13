using AutoMapper;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public class TenantFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public TenantFactory(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public object CreateMethod(string actionType)
        {
            switch (actionType.ToLower())
            {
                case "get":
                case "read":
                    return new GetData(_dbContext, _mapper);

                case "save":
                case "write":
                    return new SaveData(_dbContext, _mapper);

                case "delete":
                case "remove":
                    return new DeleteData(_dbContext);

                default:
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di TenantFactory.");
            }
        }
    }
}