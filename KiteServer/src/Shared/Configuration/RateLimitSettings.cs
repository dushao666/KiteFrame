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
    /// 通用限流规则
    /// </summary>
    public RateLimitRule[] GeneralRules { get; set; } = Array.Empty<RateLimitRule>();
}

/// <summary>
/// 限流规则
/// </summary>
public class RateLimitRule
{
    /// <summary>
    /// 端点模式（支持通配符）
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 时间周期（如：1s, 1m, 1h, 1d）
    /// </summary>
    public string Period { get; set; } = "1m";

    /// <summary>
    /// 限制次数
    /// </summary>
    public int Limit { get; set; } = 100;
}
