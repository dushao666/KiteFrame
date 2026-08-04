using HealthChecks.MySql;
using HealthChecks.Redis;

namespace Api.Extensions;

/// <summary>
/// 健康检查扩展方法
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// 添加健康检查服务（按配置决定是否检查数据库与 Redis）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
    {
        var healthCheckSettings = configuration.GetSection(HealthCheckSettings.SectionName).Get<HealthCheckSettings>() ?? new HealthCheckSettings();
        if (!healthCheckSettings.Enabled)
        {
            return services;
        }

        var builder = services.AddHealthChecks();

        // 数据库健康检查
        if (healthCheckSettings.EnableDatabaseCheck)
        {
            var connectionInfo = DatabaseConnectionFactory.Resolve(configuration);
            builder.AddMySql(connectionInfo.ConnectionString, name: "database",
                timeout: TimeSpan.FromSeconds(healthCheckSettings.TimeoutSeconds));
        }

        // Redis 健康检查（仅在启用 Redis 缓存时）
        var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>() ?? new CacheSettings();
        var redisSettings = configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>() ?? new RedisSettings();
        if (healthCheckSettings.EnableRedisCheck && cacheSettings.EnableRedisCache && redisSettings.Enabled)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? redisSettings.ConnectionString;
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                builder.AddRedis(redisConnectionString, name: "redis",
                    timeout: TimeSpan.FromSeconds(healthCheckSettings.TimeoutSeconds));
            }
        }

        return services;
    }

    /// <summary>
    /// 使用健康检查端点（/health 返回简化状态，不输出组件详情，避免泄露内部信息）
    /// </summary>
    /// <param name="app">Web应用程序</param>
    /// <returns></returns>
    public static WebApplication UseHealthCheckEndpoints(this WebApplication app)
    {
        var healthCheckSettings = app.Configuration.GetSection(HealthCheckSettings.SectionName).Get<HealthCheckSettings>() ?? new HealthCheckSettings();
        if (!healthCheckSettings.Enabled)
        {
            return app;
        }

        app.MapHealthChecks("/health");

        return app;
    }
}
