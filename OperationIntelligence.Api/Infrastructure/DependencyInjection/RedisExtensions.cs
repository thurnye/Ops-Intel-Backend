using StackExchange.Redis;

namespace OperationIntelligence.Api
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddAppRedis(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:Configuration"];
                options.InstanceName = configuration["Redis:InstanceName"];
            });

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(
                    configuration.GetConnectionString("Redis") ?? "localhost:6379"));

            return services;
        }
    }
}
