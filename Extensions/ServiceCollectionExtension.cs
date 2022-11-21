using StackExchange.Redis;

namespace HazelcastDemo.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRedisConfiguration(configuration);
            services.AddRedisConfiguration1(configuration);
            return services;
        }

        private static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<IConnectionMultiplexer>>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                try
                {
                    configuration.GetSection("Redis").Get<ConfigurationOptionsSetting>();
                    ConfigurationOptions configurationOptions = ConfigurationOptionsSetting.MapToConfigurationOptions();
                    ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
                    return connectionMultiplexer;
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                    throw;
                }
            });
            return services;
        }

        private static IServiceCollection AddRedisConfiguration1(this IServiceCollection services, IConfiguration configuration)
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<IConnectionMultiplexer>>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                try
                {
                    configuration.GetSection("Redis1").Get<ConfigurationOptionsSetting>();
                    ConfigurationOptions configurationOptions = ConfigurationOptionsSetting.MapToConfigurationOptions();
                    ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
                    return connectionMultiplexer;
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                    throw;
                }
            });
            return services;
        }
    }
}
