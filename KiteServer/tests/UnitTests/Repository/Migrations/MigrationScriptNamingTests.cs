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
    /// 命名规范正则：根前缀 + 方言目录 + 年目录（20xx） + 月目录（01-12） + V00xx__小写描述.sql
    /// （方言目录取自 DatabaseMigrator.GetDialectFolder，与迁移器过滤逻辑保持同源）
    /// </summary>
    private static Regex GetNamingRegex()
    {
        var dialects = string.Join("|",
            Enum.GetValues<DatabaseType>().Select(DatabaseMigrator.GetDialectFolder));
        return new Regex($@"^{Regex.Escape(ScriptResourceRootPrefix)}({dialects})\.(20\d{{2}})\.(0[1-9]|1[0-2])\.V\d{{4}}__[a-z0-9_]+\.sql$");
    }

    /// <summary>
    /// 解析脚本所属结构（方言目录、年目录、月目录、版本号），仅可对符合命名规范正则的资源名调用
    /// </summary>
    private static (string Dialect, string Year, string Month, string Version) ParseScriptSegments(string resourceName)
    {
        var rest = resourceName.Substring(ScriptResourceRootPrefix.Length); // 如 "MySql.2026.07.V0001__xxx.sql"
        var segments = rest.Split('.');
        return (segments[0], segments[1], segments[2], segments[3].Substring(0, "V0001".Length));
    }

    [Fact(DisplayName = "迁移脚本：已嵌入程序集且 MySQL 方言包含初始脚本 V0001")]
    public void GetManifestResourceNames_MigrationsEmbedded_ContainsV0001()
    {
        // 准备 & 执行
        var names = GetMigrationScriptNames();
        var mysqlPrefix = DatabaseMigrator.GetScriptResourcePrefix(DatabaseType.MySQL);

        // 断言（V0001 位于某个年/月日期目录下，故匹配 "方言前缀 + .V0001__"）
        Assert.NotEmpty(names);
        Assert.Contains(names, n => n.StartsWith(mysqlPrefix, StringComparison.Ordinal)
            && n.Contains(".V0001__", StringComparison.Ordinal));
    }

    [Fact(DisplayName = "迁移脚本：命名必须符合 方言目录/年/月/V00xx__小写描述.sql 规范")]
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
        // 准备：按方言分组后提取版本号
        var versionsByDialect = GetMigrationScriptNames()
            .Select(ParseScriptSegments)
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
            .Select(name => ParseScriptSegments(name).Dialect)
            .Distinct();

        // 断言
        Assert.All(dialectsInUse, dialect => Assert.Contains(dialect, knownDialects));
    }

    [Fact(DisplayName = "迁移脚本：同一方言内年月目录顺序必须与版本号顺序一致（DbUp 按资源名顺序执行）")]
    public void GetManifestResourceNames_DateFolders_ConsistentWithVersionOrder()
    {
        // 准备：按方言分组，组内按版本号升序排列
        var scriptsByDialect = GetMigrationScriptNames()
            .Select(ParseScriptSegments)
            .GroupBy(x => x.Dialect);

        // 执行 & 断言：版本号递增时，年月目录（如 202607）不允许回退，否则执行顺序将与版本号顺序不一致
        foreach (var group in scriptsByDialect)
        {
            var ordered = group.OrderBy(x => x.Version, StringComparer.Ordinal).ToList();
            for (var i = 1; i < ordered.Count; i++)
            {
                var previous = ordered[i - 1];
                var current = ordered[i];
                var comparison = string.CompareOrdinal(
                    previous.Year + previous.Month, current.Year + current.Month);
                Assert.True(comparison <= 0,
                    $"脚本 {current.Version} 的年月目录 {current.Year}/{current.Month} " +
                    $"早于前序编号脚本 {previous.Version} 的年月目录 {previous.Year}/{previous.Month}，" +
                    "会导致 DbUp 执行顺序与版本号顺序不一致");
            }
        }
    }
}
