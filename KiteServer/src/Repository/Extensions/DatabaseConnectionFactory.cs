namespace Repository.Extensions;

/// <summary>
/// 数据库连接工厂：统一解析数据库类型与连接字符串
/// </summary>
public static class DatabaseConnectionFactory
{
    /// <summary>
    /// 根据配置解析数据库连接信息
    /// </summary>
    /// <param name="configuration">应用配置</param>
    /// <returns>数据库连接信息（数据库类型、SqlSugar DbType、连接字符串）</returns>
    public static DatabaseConnectionInfo Resolve(IConfiguration configuration)
    {
        // 获取数据库配置
        var section = configuration.GetSection(DatabaseSettings.SectionName);
        var databaseTypeValue = section["DatabaseType"];
        var databaseType = string.IsNullOrEmpty(databaseTypeValue)
            ? DatabaseType.MySQL
            : Enum.Parse<DatabaseType>(databaseTypeValue);

        // 根据配置的数据库类型转换为SqlSugar的DbType和连接字符串键名
        var (dbType, connectionStringKey) = databaseType switch
        {
            DatabaseType.MySQL => (DbType.MySql, "MySQL"),
            DatabaseType.PostgreSQL => (DbType.PostgreSQL, "PostgreSQL"),
            DatabaseType.SQLServer => (DbType.SqlServer, "SQLServer"),
            DatabaseType.SQLite => (DbType.Sqlite, "SQLite"),
            DatabaseType.Oracle => (DbType.Oracle, "Oracle"),
            _ => (DbType.MySql, "MySQL") // 默认使用MySQL
        };

        // 获取对应数据库类型的连接字符串
        var connectionString = configuration.GetConnectionString(connectionStringKey)
                               ?? configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException($"未找到数据库类型 {databaseType} 对应的连接字符串配置");

        return new DatabaseConnectionInfo(databaseType, dbType, connectionString);
    }
}
