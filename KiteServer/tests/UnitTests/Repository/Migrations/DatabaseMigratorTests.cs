namespace UnitTests.Repository.Migrations;

/// <summary>
/// <see cref="DatabaseMigrator"/> 纯逻辑单元测试
/// </summary>
public class DatabaseMigratorTests
{
    [Theory(DisplayName = "迁移支持判断：当前仅 MySQL 迁移引擎已接入")]
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

    [Theory(DisplayName = "方言目录映射：各数据库类型对应 Sql/Migrations 下的约定子目录")]
    [InlineData(DatabaseType.MySQL, "MySql")]
    [InlineData(DatabaseType.PostgreSQL, "PostgreSQL")]
    [InlineData(DatabaseType.SQLServer, "SQLServer")]
    [InlineData(DatabaseType.SQLite, "SQLite")]
    [InlineData(DatabaseType.Oracle, "Oracle")]
    public void GetDialectFolder_ByDatabaseType_ReturnsConventionFolder(DatabaseType databaseType, string expected)
    {
        // 准备 & 执行
        var folder = DatabaseMigrator.GetDialectFolder(databaseType);

        // 断言
        Assert.Equal(expected, folder);
    }

    [Fact(DisplayName = "方言目录映射：未知数据库类型抛出 ArgumentOutOfRangeException")]
    public void GetDialectFolder_UnknownDatabaseType_Throws()
    {
        // 准备 & 执行 & 断言
        Assert.Throws<ArgumentOutOfRangeException>(() => DatabaseMigrator.GetDialectFolder((DatabaseType)999));
    }

    [Theory(DisplayName = "脚本资源前缀：根前缀 + 方言目录 + 点号")]
    [InlineData(DatabaseType.MySQL, "Repository.Sql.Migrations.MySql.")]
    [InlineData(DatabaseType.PostgreSQL, "Repository.Sql.Migrations.PostgreSQL.")]
    [InlineData(DatabaseType.SQLServer, "Repository.Sql.Migrations.SQLServer.")]
    [InlineData(DatabaseType.SQLite, "Repository.Sql.Migrations.SQLite.")]
    [InlineData(DatabaseType.Oracle, "Repository.Sql.Migrations.Oracle.")]
    public void GetScriptResourcePrefix_ByDatabaseType_ReturnsDialectPrefix(DatabaseType databaseType, string expected)
    {
        // 准备 & 执行
        var prefix = DatabaseMigrator.GetScriptResourcePrefix(databaseType);

        // 断言
        Assert.Equal(expected, prefix);
    }
}
