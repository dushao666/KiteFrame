namespace Infrastructure.Services;

/// <summary>
/// 内存缓存服务实现
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly CacheSettings _cacheSettings;
    private readonly ConcurrentDictionary<string, bool> _keyTracker;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="memoryCache">内存缓存</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="cacheSettings">缓存配置</param>
    public MemoryCacheService(
        IMemoryCache memoryCache,
        ILogger<MemoryCacheService> logger,
        IOptions<CacheSettings> cacheSettings)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _cacheSettings = cacheSettings.Value;
        _keyTracker = new ConcurrentDictionary<string, bool>();
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = _memoryCache.Get<T>(key);
            _logger.LogDebug("获取内存缓存，Key: {Key}, Found: {Found}", key, value != null);
            return Task.FromResult(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取内存缓存失败，Key: {Key}", key);
            return Task.FromResult<T?>(default);
        }
    }

    /// <summary>
    /// 设置缓存值
    /// </summary>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheSettings.DefaultExpirationMinutes);
                options.SlidingExpiration = TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes);
            }

            // 设置缓存项大小（当配置了SizeLimit时必须设置）
            options.Size = CalculateCacheItemSize(value);

            // 设置缓存项移除回调
            options.RegisterPostEvictionCallback((k, v, reason, state) =>
            {
                _keyTracker.TryRemove(k.ToString()!, out _);
                _logger.LogDebug("内存缓存项已移除，Key: {Key}, Reason: {Reason}", k, reason);
            });

            _memoryCache.Set(key, value, options);
            _keyTracker.TryAdd(key, true);

            _logger.LogDebug("设置内存缓存成功，Key: {Key}, Expiration: {Expiration}", key, expiration);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置内存缓存失败，Key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _memoryCache.Remove(key);
            _keyTracker.TryRemove(key, out _);
            _logger.LogDebug("删除内存缓存成功，Key: {Key}", key);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除内存缓存失败，Key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// 批量删除缓存
    /// </summary>
    public Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var key in keys)
            {
                _memoryCache.Remove(key);
                _keyTracker.TryRemove(key, out _);
            }
            _logger.LogDebug("批量删除内存缓存成功，Keys: {Keys}", string.Join(", ", keys));
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除内存缓存失败，Keys: {Keys}", string.Join(", ", keys));
            throw;
        }
    }

    /// <summary>
    /// 检查缓存是否存在
    /// </summary>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查内存缓存存在性失败，Key: {Key}", key);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// 设置缓存过期时间（内存缓存不支持动态设置过期时间）
    /// </summary>
    public Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("内存缓存不支持动态设置过期时间，Key: {Key}", key);
        return Task.FromResult(false);
    }

    /// <summary>
    /// 获取缓存剩余过期时间（内存缓存不支持获取过期时间）
    /// </summary>
    public Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("内存缓存不支持获取过期时间，Key: {Key}", key);
        return Task.FromResult<TimeSpan?>(null);
    }

    /// <summary>
    /// 根据模式删除缓存
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var keys = await GetKeysByPatternAsync(pattern, cancellationToken);
            await RemoveAsync(keys, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据模式删除内存缓存失败，Pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// 获取所有匹配模式的键
    /// </summary>
    public Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            // 简单的通配符匹配实现
            var regex = new System.Text.RegularExpressions.Regex(
                "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", ".*") + "$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            var matchingKeys = _keyTracker.Keys.Where(key => regex.IsMatch(key)).ToList();
            
            _logger.LogDebug("获取匹配模式的内存缓存键，Pattern: {Pattern}, Count: {Count}", pattern, matchingKeys.Count);
            return Task.FromResult<IEnumerable<string>>(matchingKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取匹配模式的内存缓存键失败，Pattern: {Pattern}", pattern);
            return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
        }
    }

    /// <summary>
    /// Hash操作：设置Hash字段值（内存缓存使用嵌套字典模拟）
    /// </summary>
    public Task HashSetAsync(string key, string field, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var hash = _memoryCache.Get<Dictionary<string, string>>(key) ?? new Dictionary<string, string>();
            hash[field] = value;

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheSettings.DefaultExpirationMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes),
                Size = CalculateCacheItemSize(hash)
            };

            _memoryCache.Set(key, hash, options);
            _keyTracker.TryAdd(key, true);

            _logger.LogDebug("设置内存缓存Hash字段成功，Key: {Key}, Field: {Field}", key, field);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置内存缓存Hash字段失败，Key: {Key}, Field: {Field}", key, field);
            throw;
        }
    }

    /// <summary>
    /// Hash操作：获取Hash字段值
    /// </summary>
    public Task<string?> HashGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var hash = _memoryCache.Get<Dictionary<string, string>>(key);
            var value = hash?.GetValueOrDefault(field);
            return Task.FromResult(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取内存缓存Hash字段失败，Key: {Key}, Field: {Field}", key, field);
            return Task.FromResult<string?>(null);
        }
    }

    /// <summary>
    /// Hash操作：删除Hash字段
    /// </summary>
    public Task<bool> HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var hash = _memoryCache.Get<Dictionary<string, string>>(key);
            if (hash != null && hash.Remove(field))
            {
                if (hash.Count == 0)
                {
                    _memoryCache.Remove(key);
                    _keyTracker.TryRemove(key, out _);
                }
                else
                {
                    var options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheSettings.DefaultExpirationMinutes),
                        SlidingExpiration = TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationMinutes),
                        Size = CalculateCacheItemSize(hash)
                    };
                    _memoryCache.Set(key, hash, options);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除内存缓存Hash字段失败，Key: {Key}, Field: {Field}", key, field);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Hash操作：获取Hash所有字段和值
    /// </summary>
    public Task<Dictionary<string, string>> HashGetAllAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var hash = _memoryCache.Get<Dictionary<string, string>>(key);
            return Task.FromResult(hash ?? new Dictionary<string, string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取内存缓存Hash所有字段失败，Key: {Key}", key);
            return Task.FromResult(new Dictionary<string, string>());
        }
    }

    /// <summary>
    /// 计算缓存项的大小（字节）
    /// </summary>
    /// <param name="value">缓存值</param>
    /// <returns>估算的大小（字节）</returns>
    private static long CalculateCacheItemSize<T>(T value)
    {
        if (value == null)
            return 1;

        try
        {
            // 对于字符串类型，直接计算字符串长度
            if (value is string str)
            {
                return System.Text.Encoding.UTF8.GetByteCount(str);
            }

            // 对于字典类型（Hash操作）
            if (value is Dictionary<string, string> dict)
            {
                long size = 0;
                foreach (var kvp in dict)
                {
                    size += System.Text.Encoding.UTF8.GetByteCount(kvp.Key ?? string.Empty);
                    size += System.Text.Encoding.UTF8.GetByteCount(kvp.Value ?? string.Empty);
                }
                return size > 0 ? size : 1;
            }

            // 对于其他类型，使用JSON序列化来估算大小
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            return System.Text.Encoding.UTF8.GetByteCount(json);
        }
        catch
        {
            // 如果无法计算大小，返回默认值
            return 1024; // 1KB 作为默认估算值
        }
    }
}
