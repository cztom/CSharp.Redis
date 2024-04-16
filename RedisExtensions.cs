using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CSharp.Redis
{
    /// <summary>
    /// Redis 注入扩展类
    /// </summary>
    public static class RedisExtensions
    {
        /// <summary>
        /// Redis 注入服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddOptions<RedisOptions>()
                .Configure(configuration.Bind)
                .ValidateDataAnnotations();

            services.AddSingleton<IRedisHelper, RedisHelper>();
            return services;
        }

        /// <summary>
        /// Redis 注入服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddRedis(this IServiceCollection services, Action<RedisOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);
            services.AddSingleton<IRedisHelper, RedisHelper>();
            return services;
        }
    }
}
