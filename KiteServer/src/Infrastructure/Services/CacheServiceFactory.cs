namespace Infrastructure.Services;

/// <summary>
/// 缓存服务工厂
/// </summary>
public class CacheServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CacheSettings _cacheSettings;
    private readonly RedisSettings _redisSettings;
    private readonly ILogger<CacheServiceFactory> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <param name="cacheSettings">缓存配置</param>
    /// <param name="redisSettings">Redis配置</param>
    /// <param name="logger">日志记录器</param>
    public CacheServiceFactory(
        IServiceProvider serviceProvider,
        IOptions<CacheSettings> cacheSettings,
        IOptions<RedisSettings> redisSettings,
        ILogger<CacheServiceFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _cacheSettings = cacheSettings.Value;
        _redisSettings = redisSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// 创建缓存服务实例
    /// </summary>
    /// <returns>缓存服务实例</returns>
    public ICacheService CreateCacheService()
    {
        try
        {
            // 优先使用Redis缓存（如果启用且配置正确）
            if (_cacheSettings.EnableRedisCache && _redisSettings.Enabled)
            {
                try
                {
                    var redisCacheService = _serviceProvider.GetService<RedisCacheService>();
                    if (redisCacheService != null)
                    {
                        _logger.LogInformation("使用Redis缓存服务");
                        return redisCacheService;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Redis缓存服务创建失败，降级使用内存缓存");
                }
            }

            // 降级使用内存缓存
            if (_cacheSettings.EnableMemoryCache)
            {
                var memoryCacheService = _serviceProvider.GetService<MemoryCacheService>();
                if (memoryCacheService != null)
                {
                    _logger.LogInformation("使用内存缓存服务");
                    return memoryCacheService;
                }
            }

            _logger.LogWarning("所有缓存服务都不可用，返回空缓存服务");
            return new NullCacheService();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建缓存服务失败，返回空缓存服务");
            return new NullCacheService();
        }
    }
}

/// <summary>
/// 空缓存服务实现（当所有缓存都不可用时使用）
/// </summary>
public class NullCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<TimeSpan?>(null);
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
    }

    public Task HashSetAsync(string key, string field, string value, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<string?> HashGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(null);
    }

    public Task<bool> HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<Dictionary<string, string>> HashGetAllAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Dictionary<string, string>());
    }
}
