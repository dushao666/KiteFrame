using System.Text.RegularExpressions;
using Repository;
using Repository.Migrations;
using Shared.Configuration;

namespace UnitTests.Repository.Migrations;

/// <summary>
/// 迁移脚本嵌入资源命名规范守护测试
/// </summary>
public class MigrationScriptNamingTests
{
    /// <summary>
    /// 迁移脚本嵌入资源根前缀（与 DatabaseMigrator 保持一致，对应 Sql/Migrations 目录）
    /// </summary>
    private const string ScriptResourceRootPrefix = "Repository.Sql.Migrations.";

    private static List<string> GetMigrationScriptNames() =>
        typeof(DBContext).Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(ScriptResourceRootPrefix, StringComparison.Ordinal))
            .ToList();

    /// <summary>
    /// 命名规范正则：根前缀 + 方言目录 + V00xx__小写描述.sql
    /// （方言目录取自 DatabaseMigrator.GetDialectFolder，与迁移器过滤逻辑保持同源）
    /// </summary>
    private static Regex GetNamingRegex()
    {
        var dialects = string.Join("|",
            Enum.GetValues<DatabaseType>().Select(DatabaseMigrator.GetDialectFolder));
        return new Regex($@"^{Regex.Escape(ScriptResourceRootPrefix)}({dialects})\.V\d{{4}}__[a-z0-9_]+\.sql$");
    }

    /// <summary>
    /// 提取脚本所属方言目录名（根前缀之后、脚本文件名之前的部分）
    /// </summary>
    private static string GetDialectSegment(string resourceName) =>
        resourceName.Substring(ScriptResourceRootPrefix.Length)
            .Split('.', 2)[0];

    [Fact(DisplayName = "迁移脚本：已嵌入程序集且 MySQL 方言包含初始脚本 V0001")]
    public void GetManifestResourceNames_MigrationsEmbedded_ContainsV0001()
    {
        // 准备 & 执行
        var names = GetMigrationScriptNames();
        var mysqlV0001Prefix = DatabaseMigrator.GetScriptResourcePrefix(DatabaseType.MySQL) + "V0001__";

        // 断言
        Assert.NotEmpty(names);
        Assert.Contains(names, n => n.StartsWith(mysqlV0001Prefix, StringComparison.Ordinal));
    }

    [Fact(DisplayName = "迁移脚本：命名必须符合 方言目录/V00xx__小写描述.sql 规范")]
    public void GetManifestResourceNames_AllScripts_MatchNamingConvention()
    {
        // 准备
        var names = GetMigrationScriptNames();
        var regex = GetNamingRegex();

        // 执行 & 断言
        Assert.All(names, name => Assert.Matches(regex, name));
    }

    [Fact(DisplayName = "迁移脚本：同一方言内版本号不允许重复（不同方言可各自从 V0001 起编）")]
    public void GetManifestResourceNames_VersionNumbers_NoDuplicatesWithinDialect()
    {
        // 准备：按方言分组后提取版本号（"V0001"，即方言段之后的 5 个字符）
        var versionsByDialect = GetMigrationScriptNames()
            .Select(name =>
            {
                var rest = name.Substring(ScriptResourceRootPrefix.Length); // "MySql.V0001__xxx.sql"
                var dialect = rest.Split('.', 2)[0];
                var version = rest.Substring(dialect.Length + 1, "V0001".Length);
                return (Dialect: dialect, Version: version);
            })
            .GroupBy(x => x.Dialect);

        // 执行 & 断言
        Assert.All(versionsByDialect, group =>
            Assert.Equal(group.Count(), group.Select(x => x.Version).Distinct().Count()));
    }

    [Fact(DisplayName = "迁移脚本：嵌入顺序与文件名排序一致（DbUp 按名称顺序执行）")]
    public void GetManifestResourceNames_Order_MatchesOrdinalSort()
    {
        // 准备
        var names = GetMigrationScriptNames();

        // 执行
        var sorted = names.OrderBy(name => name, StringComparer.Ordinal).ToList();

        // 断言
        Assert.Equal(sorted, names);
    }

    [Fact(DisplayName = "迁移脚本：方言目录必须与 DatabaseMigrator 的方言映射一致")]
    public void GetManifestResourceNames_DialectSegments_MatchMigratorMapping()
    {
        // 准备
        var knownDialects = Enum.GetValues<DatabaseType>()
            .Select(DatabaseMigrator.GetDialectFolder)
            .ToHashSet(StringComparer.Ordinal);

        // 执行
        var dialectsInUse = GetMigrationScriptNames()
            .Select(GetDialectSegment)
            .Distinct();

        // 断言
        Assert.All(dialectsInUse, dialect => Assert.Contains(dialect, knownDialects));
    }
}
