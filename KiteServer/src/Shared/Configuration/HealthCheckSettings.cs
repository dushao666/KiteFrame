namespace Shared.Configuration;

/// <summary>
/// 健康检查配置设置
/// </summary>
public class HealthCheckSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "HealthCheckSettings";

    /// <summary>
    /// 是否启用健康检查
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 是否启用数据库健康检查
    /// </summary>
    public bool EnableDatabaseCheck { get; set; } = true;

    /// <summary>
    /// 是否启用 Redis 健康检查
    /// </summary>
    public bool EnableRedisCheck { get; set; } = false;

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// 是否显示详细错误信息
    /// </summary>
    public bool DetailedErrors { get; set; } = false;
}
