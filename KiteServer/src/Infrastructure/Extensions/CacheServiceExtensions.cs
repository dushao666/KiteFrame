namespace Infrastructure.Extensions;

/// <summary>
/// 缓存服务扩展方法
/// </summary>
public static class CacheServiceExtensions
{
    /// <summary>
    /// 添加缓存服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册配置
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));

        var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>() ?? new CacheSettings();
        var redisSettings = configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>() ?? new RedisSettings();

        // 添加内存缓存
        if (cacheSettings.EnableMemoryCache)
        {
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = cacheSettings.MaxMemoryCacheSizeMB * 1024 * 1024; // 转换为字节
            });
            services.AddScoped<MemoryCacheService>();
        }

        // 添加Redis缓存
        if (cacheSettings.EnableRedisCache && redisSettings.Enabled)
        {
            try
            {
                // 从连接字符串配置中获取Redis连接字符串
                var redisConnectionString = configuration.GetConnectionString("Redis");
                if (string.IsNullOrEmpty(redisConnectionString))
                {
                    redisConnectionString = redisSettings.ConnectionString;
                }

                // 配置Redis连接
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<IConnectionMultiplexer>>();
                    
                    try
                    {
                        var configuration = ConfigurationOptions.Parse(redisConnectionString);
                        configuration.ConnectTimeout = redisSettings.ConnectTimeout;
                        configuration.SyncTimeout = redisSettings.SyncTimeout;
                        configuration.CommandMap = CommandMap.Create(new HashSet<string>
                        {
                            // 如果需要禁用某些Redis命令，可以在这里添加
                        }, available: false);

                        var multiplexer = ConnectionMultiplexer.Connect(configuration);
                        
                        // 添加连接事件处理
                        multiplexer.ConnectionFailed += (sender, e) =>
                        {
                            logger.LogError("Redis连接失败: {EndPoint} - {FailureType} - {Exception}", 
                                e.EndPoint, e.FailureType, e.Exception?.Message);
                        };

                        multiplexer.ConnectionRestored += (sender, e) =>
                        {
                            logger.LogInformation("Redis连接已恢复: {EndPoint}", e.EndPoint);
                        };

                        multiplexer.ErrorMessage += (sender, e) =>
                        {
                            logger.LogError("Redis错误消息: {EndPoint} - {Message}", e.EndPoint, e.Message);
                        };

                        logger.LogInformation("Redis连接成功建立: {ConnectionString}", redisConnectionString);
                        return multiplexer;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Redis连接失败: {ConnectionString}", redisConnectionString);
                        throw;
                    }
                });

                // 注释掉分布式缓存配置，只使用自定义Redis服务
                // services.AddStackExchangeRedisCache(options =>
                // {
                //     options.Configuration = redisConnectionString;
                //     options.InstanceName = redisSettings.InstanceName;
                // });

                services.AddScoped<RedisCacheService>();
            }
            catch (Exception ex)
            {
                var serviceProvider = services.BuildServiceProvider();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("CacheServiceExtensions");
                logger?.LogError(ex, "Redis缓存配置失败，将禁用Redis缓存");
            }
        }

        // 注册缓存服务工厂
        services.AddScoped<CacheServiceFactory>();

        // 注册主缓存服务
        services.AddScoped<Services.ICacheService>(provider =>
        {
            var factory = provider.GetRequiredService<CacheServiceFactory>();
            return factory.CreateCacheService();
        });

        return services;
    }

    /// <summary>
    /// 生成缓存键
    /// </summary>
    /// <param name="prefix">前缀</param>
    /// <param name="keys">键值</param>
    /// <returns></returns>
    public static string GenerateCacheKey(string prefix, params object[] keys)
    {
        if (keys == null || keys.Length == 0)
            return prefix;

        var keyString = string.Join(":", keys.Select(k => k?.ToString() ?? "null"));
        return $"{prefix}:{keyString}";
    }

    /// <summary>
    /// 生成用户相关的缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="suffix">后缀</param>
    /// <returns></returns>
    public static string GenerateUserCacheKey(long userId, string suffix)
    {
        return GenerateCacheKey("user", userId, suffix);
    }

    /// <summary>
    /// 生成JWT令牌相关的缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="tokenType">令牌类型</param>
    /// <returns></returns>
    public static string GenerateTokenCacheKey(long userId, string tokenType)
    {
        return GenerateCacheKey("token", userId, tokenType);
    }

    /// <summary>
    /// 生成登录会话相关的缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="sessionId">会话ID</param>
    /// <returns></returns>
    public static string GenerateSessionCacheKey(long userId, string sessionId)
    {
        return GenerateCacheKey("session", userId, sessionId);
    }
}
