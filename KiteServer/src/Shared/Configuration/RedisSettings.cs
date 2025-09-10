namespace Shared.Configuration;

/// <summary>
/// Redis 配置设置
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "RedisSettings";

    /// <summary>
    /// 是否启用 Redis
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// 数据库索引
    /// </summary>
    public int Database { get; set; } = 0;

    /// <summary>
    /// 实例名称
    /// </summary>
    public string InstanceName { get; set; } = "KiteServer";

    /// <summary>
    /// 命令超时时间（毫秒）
    /// </summary>
    public int CommandTimeout { get; set; } = 5000;

    /// <summary>
    /// 连接超时时间（毫秒）
    /// </summary>
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    /// 同步超时时间（毫秒）
    /// </summary>
    public int SyncTimeout { get; set; } = 5000;
}
