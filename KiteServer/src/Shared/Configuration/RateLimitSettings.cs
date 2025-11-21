namespace Shared.Configuration;

/// <summary>
/// 限流配置设置
/// </summary>
public class RateLimitSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "RateLimitSettings";

    /// <summary>
    /// 是否启用限流
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 全局限流策略类型：
    /// - Fixed（固定窗口）：在固定的时间窗口内限制请求数量，窗口重置时计数器归零。适合简单场景，但可能出现突发流量
    /// - Sliding（滑动窗口）：使用滑动时间窗口，将时间窗口分成多个小段，更平滑地限制流量，避免固定窗口边界的突发问题
    /// - Token（令牌桶）：按固定速率补充令牌，请求消耗令牌，允许一定程度的突发流量（桶的容量）。适合需要控制平均速率的场景
    /// - Concurrency（并发限制）：限制同时处理的请求数量，不考虑时间因素。适合保护资源密集型操作
    /// </summary>
    public string PolicyType { get; set; } = "Fixed";

    /// <summary>
    /// 固定窗口限流配置
    /// </summary>
    public FixedWindowConfig? FixedWindow { get; set; }

    /// <summary>
    /// 滑动窗口限流配置
    /// </summary>
    public SlidingWindowConfig? SlidingWindow { get; set; }

    /// <summary>
    /// 令牌桶限流配置
    /// </summary>
    public TokenBucketConfig? TokenBucket { get; set; }

    /// <summary>
    /// 并发限流配置
    /// </summary>
    public ConcurrencyConfig? Concurrency { get; set; }

    /// <summary>
    /// 是否启用IP限流
    /// </summary>
    public bool EnableIpRateLimit { get; set; } = true;

    /// <summary>
    /// 是否启用用户限流
    /// </summary>
    public bool EnableUserRateLimit { get; set; } = false;

    /// <summary>
    /// HTTP状态码（当请求被限流时返回）
    /// </summary>
    public int StatusCode { get; set; } = 429;

    /// <summary>
    /// 限流提示消息
    /// </summary>
    public string Message { get; set; } = "请求过于频繁，请稍后再试";
}

/// <summary>
/// 固定窗口限流配置
/// </summary>
public class FixedWindowConfig
{
    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int Window { get; set; } = 60;

    /// <summary>
    /// 请求限制数量
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// 队列限制
    /// </summary>
    public int QueueLimit { get; set; } = 0;
}

/// <summary>
/// 滑动窗口限流配置
/// </summary>
public class SlidingWindowConfig
{
    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int Window { get; set; } = 60;

    /// <summary>
    /// 请求限制数量
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// 滑动窗口分段数量
    /// </summary>
    public int SegmentsPerWindow { get; set; } = 6;

    /// <summary>
    /// 队列限制
    /// </summary>
    public int QueueLimit { get; set; } = 0;
}

/// <summary>
/// 令牌桶限流配置
/// </summary>
public class TokenBucketConfig
{
    /// <summary>
    /// 令牌桶容量
    /// </summary>
    public int TokenLimit { get; set; } = 100;

    /// <summary>
    /// 每个周期补充的令牌数量
    /// </summary>
    public int TokensPerPeriod { get; set; } = 10;

    /// <summary>
    /// 补充周期（秒）
    /// </summary>
    public int ReplenishmentPeriod { get; set; } = 10;

    /// <summary>
    /// 队列限制
    /// </summary>
    public int QueueLimit { get; set; } = 0;
}

/// <summary>
/// 并发限流配置
/// </summary>
public class ConcurrencyConfig
{
    /// <summary>
    /// 并发请求限制数量
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// 队列限制
    /// </summary>
    public int QueueLimit { get; set; } = 0;
}
