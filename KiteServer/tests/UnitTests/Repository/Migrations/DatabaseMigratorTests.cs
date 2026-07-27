using Repository.Migrations;
using Shared.Configuration;

namespace UnitTests.Repository.Migrations;

/// <summary>
/// <see cref="DatabaseMigrator"/> 纯逻辑单元测试
/// </summary>
public class DatabaseMigratorTests
{
    [Theory(DisplayName = "迁移支持判断：仅 MySQL 受支持")]
    [InlineData(DatabaseType.MySQL, true)]
    [InlineData(DatabaseType.PostgreSQL, false)]
    [InlineData(DatabaseType.SQLServer, false)]
    [InlineData(DatabaseType.SQLite, false)]
    [InlineData(DatabaseType.Oracle, false)]
    public void IsMigrationSupported_ByDatabaseType_OnlyMySQLSupported(DatabaseType databaseType, bool expected)
    {
        // 准备 & 执行
        var supported = DatabaseMigrator.IsMigrationSupported(databaseType);

        // 断言
        Assert.Equal(expected, supported);
    }
}
