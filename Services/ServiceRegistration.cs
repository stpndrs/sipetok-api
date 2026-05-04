using sipetok_api.service;

namespace sipetok_api.Services
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<PaymentService>();
        }
    }
}
