using AutoMapper;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.Services;

namespace sipetok_api.Controllers.Factories
{
    public class TransactionFactory : StevanModuleFactory
    {
        private readonly IConfiguration appConfig;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly PaymentService _paymentService;
        private readonly OrderService _orderService;

        public TransactionFactory(AppDbContext dbContext, IConfiguration config, IMapper mapper, PaymentService paymentService, OrderService orderService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _paymentService = paymentService;
            _orderService = orderService;
            appConfig = config;
        }

        public IStevanMethod CreateMethod(string actionType)
        {
            string action = actionType?.ToLower()?.Trim() ?? string.Empty;

            switch (action)
            {
                case "get":
                case "read":
                    return new StevanGetData(_dbContext, _mapper);

                case "save":
                case "write":
                    return new StevanSaveData(_dbContext, appConfig, _mapper, _paymentService, _orderService);

                default:
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di TransactionFactory.");
            }
        }
    }
}