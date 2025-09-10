using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using SqlSugar;

namespace Infrastructure.Extensions;

/// <summary>
/// 服务集合扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加基础设施服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 添加配置选项
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));

        // 添加 SqlSugar
        services.AddSqlSugar(configuration);

        // 添加缓存服务
        services.AddCaching(configuration);

        // 添加仓储和工作单元
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// 添加 SqlSugar 服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册 SqlSugarContext
        services.AddSingleton<SqlSugarContext>();

        // 注册 ISqlSugarClient
        services.AddScoped<ISqlSugarClient>(provider =>
        {
            var context = provider.GetRequiredService<SqlSugarContext>();
            return context.CreateClient();
        });

        return services;
    }

    /// <summary>
    /// 添加缓存服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>();
        var redisSettings = configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();

        // 添加内存缓存
        if (cacheSettings?.EnableMemoryCache == true)
        {
            services.AddMemoryCache(options =>
            {
                if (cacheSettings.MaxMemoryCacheSizeMB > 0)
                {
                    options.SizeLimit = cacheSettings.MaxMemoryCacheSizeMB * 1024 * 1024; // 转换为字节
                }
            });
        }

        // 添加 Redis 缓存
        if (cacheSettings?.EnableRedisCache == true && redisSettings?.Enabled == true)
        {
            var redisConnectionString = redisSettings.ConnectionString;
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                // 暂时注释掉，需要添加 StackExchange.Redis 包
                // services.AddStackExchangeRedisCache(options =>
                // {
                //     options.Configuration = redisConnectionString;
                //     options.InstanceName = redisSettings.InstanceName;
                // });
            }
        }

        return services;
    }

    /// <summary>
    /// 添加认证服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT 配置将在后续实现
        return services;
    }

    /// <summary>
    /// 添加日志服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        // Serilog 配置将在后续实现
        return services;
    }
}
