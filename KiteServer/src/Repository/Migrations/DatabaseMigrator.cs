using DbUp;
using Repository.Extensions;

namespace Repository.Migrations;

/// <summary>
/// 数据库迁移执行器（基于 DbUp，Flyway 式 SQL 脚本迁移）
/// </summary>
public static class DatabaseMigrator
{
    /// <summary>
    /// 迁移脚本嵌入资源根前缀（对应 Sql/Migrations 目录）
    /// </summary>
    private const string ScriptResourceRootPrefix = "Repository.Sql.Migrations.";

    /// <summary>
    /// 迁移版本表（记录已执行脚本，DbUp 自动创建）
    /// </summary>
    private const string JournalTableName = "sys_schema_versions";

    /// <summary>
    /// 执行数据库迁移：按脚本名顺序运行当前数据库方言目录下所有尚未执行的嵌入式迁移脚本
    /// </summary>
    /// <param name="configuration">应用配置</param>
    public static void MigrateDatabase(IConfiguration configuration)
    {
        // 解析数据库类型与连接字符串（与数据库装配共用同一解析逻辑）
        var connection = DatabaseConnectionFactory.Resolve(configuration);

        // 当前仅接入 MySQL 迁移引擎；其他数据库类型跳过，不阻断启动
        if (!IsMigrationSupported(connection.DatabaseType))
        {
            Log.Warning("数据库类型 {DatabaseType} 暂不支持自动迁移，已跳过数据库迁移", connection.DatabaseType);
            return;
        }

        Log.Information("开始执行数据库迁移");

        // 数据库不存在时自动创建（幂等）
        EnsureDatabase.For.MySqlDatabase(connection.ConnectionString);

        // 按方言前缀过滤，仅执行当前数据库类型对应目录下的脚本（如 Repository.Sql.Migrations.MySql.*）
        var scriptPrefix = GetScriptResourcePrefix(connection.DatabaseType);

        var upgrader = DeployChanges.To
            .MySqlDatabase(connection.ConnectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DBContext).Assembly,
                name => name.StartsWith(scriptPrefix, StringComparison.Ordinal))
            .JournalToMySqlTable(null, JournalTableName)
            .LogTo(new SerilogUpgradeLog())
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Log.Error(result.Error, "数据库迁移失败，失败脚本：{ScriptName}", result.ErrorScript?.Name);
            throw new InvalidOperationException("数据库迁移失败", result.Error);
        }

        Log.Information("数据库迁移完成，本次执行 {ScriptCount} 个脚本", result.Scripts.Count());
    }

    /// <summary>
    /// 判断指定数据库类型是否支持自动迁移（是否已接入对应的 DbUp 迁移引擎）
    /// </summary>
    /// <param name="databaseType">配置的数据库类型</param>
    /// <returns>当前仅 MySQL 返回 true</returns>
    public static bool IsMigrationSupported(DatabaseType databaseType) => databaseType == DatabaseType.MySQL;

    /// <summary>
    /// 获取指定数据库类型的迁移脚本嵌入资源前缀（方言目录）
    /// </summary>
    /// <param name="databaseType">配置的数据库类型</param>
    /// <returns>如 MySQL 对应 "Repository.Sql.Migrations.MySql."</returns>
    public static string GetScriptResourcePrefix(DatabaseType databaseType) =>
        ScriptResourceRootPrefix + GetDialectFolder(databaseType) + ".";

    /// <summary>
    /// 获取指定数据库类型对应的迁移脚本方言目录名（与 Sql/Migrations 下的子目录一一对应）
    /// </summary>
    /// <param name="databaseType">配置的数据库类型</param>
    /// <returns>方言目录名，如 "MySql"、"PostgreSQL"</returns>
    /// <exception cref="ArgumentOutOfRangeException">未知的数据库类型</exception>
    public static string GetDialectFolder(DatabaseType databaseType) => databaseType switch
    {
        DatabaseType.MySQL => "MySql",
        DatabaseType.PostgreSQL => "PostgreSQL",
        DatabaseType.SQLServer => "SQLServer",
        DatabaseType.SQLite => "SQLite",
        DatabaseType.Oracle => "Oracle",
        _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, "未知的数据库类型，无对应的迁移脚本方言目录")
    };
}
