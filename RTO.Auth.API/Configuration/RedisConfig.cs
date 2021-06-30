using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RTO.Auth.API.Configuration
{
    public static class RedisConfig
    {
        public static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisCs");
                options.InstanceName = "AuthDB-";
            });

            return services;
        }

    }
}