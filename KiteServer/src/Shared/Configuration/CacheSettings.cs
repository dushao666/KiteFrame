namespace Shared.Configuration;

/// <summary>
/// 缓存配置设置
/// </summary>
public class CacheSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "CacheSettings";

    /// <summary>
    /// 是否启用内存缓存
    /// </summary>
    public bool EnableMemoryCache { get; set; } = true;

    /// <summary>
    /// 是否启用 Redis 缓存
    /// </summary>
    public bool EnableRedisCache { get; set; } = false;

    /// <summary>
    /// 默认过期时间（分钟）
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// 滑动过期时间（分钟）
    /// </summary>
    public int SlidingExpirationMinutes { get; set; } = 10;

    /// <summary>
    /// 内存缓存最大大小（MB）
    /// </summary>
    public int MaxMemoryCacheSizeMB { get; set; } = 100;
}
