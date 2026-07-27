namespace Infrastructure.Services;

/// <summary>
/// Redis缓存服务实现
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly CacheSettings _cacheSettings;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="connectionMultiplexer">Redis连接</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="cacheSettings">缓存配置</param>
    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger,
        IOptions<CacheSettings> cacheSettings)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
        _cacheSettings = cacheSettings.Value;
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue)
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)value.ToString();

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取缓存失败，Key: {Key}", key);
            return default;
        }
    }

    /// <summary>
    /// 设置缓存值
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serializedValue = typeof(T) == typeof(string) 
                ? value?.ToString() 
                : JsonSerializer.Serialize(value);

            var expirationTime = expiration ?? TimeSpan.FromMinutes(_cacheSettings.DefaultExpirationMinutes);
            
            await _database.StringSetAsync(key, serializedValue, expirationTime);
            
            _logger.LogDebug("设置缓存成功，Key: {Key}, Expiration: {Expiration}", key, expirationTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置缓存失败，Key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogDebug("删除缓存成功，Key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除缓存失败，Key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// 批量删除缓存
    /// </summary>
    public async Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            await _database.KeyDeleteAsync(redisKeys);
            _logger.LogDebug("批量删除缓存成功，Keys: {Keys}", string.Join(", ", keys));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除缓存失败，Keys: {Keys}", string.Join(", ", keys));
            throw;
        }
    }

    /// <summary>
    /// 检查缓存是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查缓存存在性失败，Key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 设置缓存过期时间
    /// </summary>
    public async Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.KeyExpireAsync(key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置缓存过期时间失败，Key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 获取缓存剩余过期时间
    /// </summary>
    public async Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.KeyTimeToLiveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取缓存过期时间失败，Key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// 根据模式删除缓存
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var keys = await GetKeysByPatternAsync(pattern, cancellationToken);
            if (keys.Any())
            {
                await RemoveAsync(keys, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据模式删除缓存失败，Pattern: {Pattern}", pattern);
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
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);
            return Task.FromResult<IEnumerable<string>>(keys.Select(k => k.ToString()).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取匹配模式的键失败，Pattern: {Pattern}", pattern);
            return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
        }
    }

    /// <summary>
    /// Hash操作：设置Hash字段值
    /// </summary>
    public async Task HashSetAsync(string key, string field, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.HashSetAsync(key, field, value);
            _logger.LogDebug("设置Hash字段成功，Key: {Key}, Field: {Field}", key, field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置Hash字段失败，Key: {Key}, Field: {Field}", key, field);
            throw;
        }
    }

    /// <summary>
    /// Hash操作：获取Hash字段值
    /// </summary>
    public async Task<string?> HashGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.HashGetAsync(key, field);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取Hash字段失败，Key: {Key}, Field: {Field}", key, field);
            return null;
        }
    }

    /// <summary>
    /// Hash操作：删除Hash字段
    /// </summary>
    public async Task<bool> HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.HashDeleteAsync(key, field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除Hash字段失败，Key: {Key}, Field: {Field}", key, field);
            return false;
        }
    }

    /// <summary>
    /// Hash操作：获取Hash所有字段和值
    /// </summary>
    public async Task<Dictionary<string, string>> HashGetAllAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var hashFields = await _database.HashGetAllAsync(key);
            return hashFields.ToDictionary(
                field => field.Name.ToString(),
                field => field.Value.ToString()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取Hash所有字段失败，Key: {Key}", key);
            return new Dictionary<string, string>();
        }
    }
}
