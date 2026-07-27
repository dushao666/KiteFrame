namespace Shared.Configuration;

/// <summary>
/// 数据库配置设置
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "DatabaseSettings";

    /// <summary>
    /// 数据库类型：0=MySQL, 1=PostgreSQL, 2=SQLServer, 3=SQLite, 4=Oracle
    /// </summary>
    public DatabaseType DatabaseType { get; set; } = DatabaseType.MySQL;

    /// <summary>
    /// 命令超时时间（秒）
    /// </summary>
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// 是否启用 SQL 日志
    /// </summary>
    public bool EnableSqlLog { get; set; } = false;

    /// <summary>
    /// 是否启用 CodeFirst（自动建表）
    /// </summary>
    public bool EnableCodeFirst { get; set; } = true;

    /// <summary>
    /// 是否启用软删除
    /// </summary>
    public bool EnableSoftDelete { get; set; } = true;

    /// <summary>
    /// 表名前缀
    /// </summary>
    public string TablePrefix { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用审计字段（创建时间、更新时间等）
    /// </summary>
    public bool EnableAuditFields { get; set; } = true;
}

/// <summary>
/// 数据库类型枚举
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// MySQL
    /// </summary>
    MySQL = 0,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSQL = 1,

    /// <summary>
    /// SQL Server
    /// </summary>
    SQLServer = 2,

    /// <summary>
    /// SQLite
    /// </summary>
    SQLite = 3,

    /// <summary>
    /// Oracle
    /// </summary>
    Oracle = 4
}
