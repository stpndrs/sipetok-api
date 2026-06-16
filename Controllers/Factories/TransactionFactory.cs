using AutoMapper;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.Services;

namespace sipetok_api.Controllers.Factories
{
    public class TransactionFactory : ModuleFactory
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly PaymentService _paymentService;
        private readonly OrderService _orderService;

        public TransactionFactory(AppDbContext dbContext, IMapper mapper, PaymentService paymentService, OrderService orderService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public IMethod CreateMethod(string actionType)
        {
            // Menghindari NullReferenceException dan meratakan teks ke huruf kecil
            string action = actionType?.ToLower()?.Trim() ?? string.Empty;

            switch (action)
            {
                case "get":
                case "read":
                    return new GetData(_dbContext, _mapper);

                case "save":
                case "write":
                    return new SaveData(_dbContext, _mapper, _paymentService, _orderService);

                case "delete":
                case "remove":
                    return new DeleteData(_dbContext);

                default:
                    throw new ArgumentException($"Aksi '{actionType}' tidak dikenal di TransactionFactory.");
            }
        }
    }
}