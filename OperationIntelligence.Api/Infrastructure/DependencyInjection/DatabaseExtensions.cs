using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Api
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddAppDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<OperationIntelligenceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
