using Microsoft.Extensions.Configuration;
using Repository.Extensions;
using Shared.Configuration;
using SqlSugar;

namespace UnitTests.Repository.Extensions;

/// <summary>
/// <see cref="DatabaseConnectionFactory"/> 单元测试
/// </summary>
public class DatabaseConnectionFactoryTests
{
    private static IConfiguration BuildConfiguration(string? databaseType, params (string Key, string Value)[] connectionStrings)
    {
        var settings = new Dictionary<string, string?>();
        if (databaseType is not null)
        {
            settings["DatabaseSettings:DatabaseType"] = databaseType;
        }
        foreach (var (key, value) in connectionStrings)
        {
            settings[$"ConnectionStrings:{key}"] = value;
        }
        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }

    [Theory(DisplayName = "解析：各数据库类型映射到对应的 DbType")]
    [InlineData("0", DbType.MySql, "MySQL")]
    [InlineData("1", DbType.PostgreSQL, "PostgreSQL")]
    [InlineData("2", DbType.SqlServer, "SQLServer")]
    [InlineData("3", DbType.Sqlite, "SQLite")]
    [InlineData("4", DbType.Oracle, "Oracle")]
    public void Resolve_KnownDatabaseType_MapsToExpectedDbType(string databaseType, DbType expectedDbType, string expectedKey)
    {
        // 准备
        var configuration = BuildConfiguration(databaseType, (expectedKey, "conn-value"));

        // 执行
        var info = DatabaseConnectionFactory.Resolve(configuration);

        // 断言
        Assert.Equal(expectedDbType, info.DbType);
        Assert.Equal("conn-value", info.ConnectionString);
    }

    [Fact(DisplayName = "解析：未配置 DatabaseType 时默认 MySQL")]
    public void Resolve_DatabaseTypeMissing_DefaultsToMySQL()
    {
        // 准备
        var configuration = BuildConfiguration(null, ("MySQL", "conn-value"));

        // 执行
        var info = DatabaseConnectionFactory.Resolve(configuration);

        // 断言
        Assert.Equal(DatabaseType.MySQL, info.DatabaseType);
        Assert.Equal(DbType.MySql, info.DbType);
    }

    [Fact(DisplayName = "解析：缺少类型专用连接串时回退 DefaultConnection")]
    public void Resolve_TypeSpecificKeyMissing_FallsBackToDefaultConnection()
    {
        // 准备
        var configuration = BuildConfiguration("0", ("DefaultConnection", "fallback-value"));

        // 执行
        var info = DatabaseConnectionFactory.Resolve(configuration);

        // 断言
        Assert.Equal("fallback-value", info.ConnectionString);
    }

    [Fact(DisplayName = "解析：完全未配置连接串时抛出异常")]
    public void Resolve_NoConnectionStrings_Throws()
    {
        // 准备
        var configuration = BuildConfiguration("0");

        // 执行 & 断言
        Assert.Throws<InvalidOperationException>(() => DatabaseConnectionFactory.Resolve(configuration));
    }
}
