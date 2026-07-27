using System.Reflection;
using System.Text.RegularExpressions;
using Repository;

namespace UnitTests.Repository.Migrations;

/// <summary>
/// 迁移脚本嵌入资源命名规范守护测试
/// </summary>
public class MigrationScriptNamingTests
{
    /// <summary>
    /// 迁移脚本嵌入资源前缀（与 DatabaseMigrator 保持一致）
    /// </summary>
    private const string ScriptResourcePrefix = "Repository.Sql.Migrations.";

    private static List<string> GetMigrationScriptNames() =>
        typeof(DBContext).Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(ScriptResourcePrefix, StringComparison.Ordinal))
            .ToList();

    [Fact(DisplayName = "迁移脚本：已嵌入程序集且包含初始脚本 V0001")]
    public void GetManifestResourceNames_MigrationsEmbedded_ContainsV0001()
    {
        // 准备 & 执行
        var names = GetMigrationScriptNames();

        // 断言
        Assert.NotEmpty(names);
        Assert.Contains(names, n => n.StartsWith(ScriptResourcePrefix + "V0001__", StringComparison.Ordinal));
    }

    [Fact(DisplayName = "迁移脚本：命名必须符合 V00xx__小写描述.sql 规范")]
    public void GetManifestResourceNames_AllScripts_MatchNamingConvention()
    {
        // 准备
        var names = GetMigrationScriptNames();
        var regex = new Regex(@"^Repository\.Sql\.Migrations\.V\d{4}__[a-z0-9_]+\.sql$");

        // 执行 & 断言
        Assert.All(names, name => Assert.Matches(regex, name));
    }

    [Fact(DisplayName = "迁移脚本：版本号不允许重复")]
    public void GetManifestResourceNames_VersionNumbers_NoDuplicates()
    {
        // 准备
        var versions = GetMigrationScriptNames()
            .Select(name => name.Substring(ScriptResourcePrefix.Length, 5)) // "V0001"
            .ToList();

        // 执行 & 断言
        Assert.Equal(versions.Count, versions.Distinct().Count());
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
}
