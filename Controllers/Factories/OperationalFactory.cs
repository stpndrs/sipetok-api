using AutoMapper;
using sipetok_api.Data;
using sipetok_api.Controllers.Products;

namespace sipetok_api.Controllers.Factories
{
    public class OperationalFactory : ModuleFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public OperationalFactory(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IMethod CreateMethod(string actionType)
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
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di OperationalFactory.");
            }
        }
    }
}