namespace Shared.Configuration;

/// <summary>
/// Swagger 配置
/// </summary>
public class SwaggerSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "SwaggerSettings";

    /// <summary>
    /// 是否启用 Swagger
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// API 标题
    /// </summary>
    public string Title { get; set; } = "KiteServer API";

    /// <summary>
    /// API 描述
    /// </summary>
    public string Description { get; set; } = "KiteServer 后端管理系统 API 文档";

    /// <summary>
    /// API 版本
    /// </summary>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// 联系人信息
    /// </summary>
    public ContactInfo Contact { get; set; } = new();

    /// <summary>
    /// 许可证信息
    /// </summary>
    public LicenseInfo License { get; set; } = new();

    /// <summary>
    /// 服务器信息
    /// </summary>
    public List<ServerInfo> Servers { get; set; } = new();

    /// <summary>
    /// 是否启用 JWT 认证
    /// </summary>
    public bool EnableJwtAuth { get; set; } = true;

    /// <summary>
    /// 是否启用 XML 注释
    /// </summary>
    public bool EnableXmlComments { get; set; } = true;

    /// <summary>
    /// XML 注释文件路径
    /// </summary>
    public List<string> XmlCommentFiles { get; set; } = new();

    /// <summary>
    /// 是否启用注解
    /// </summary>
    public bool EnableAnnotations { get; set; } = true;

    /// <summary>
    /// 是否在生产环境启用
    /// </summary>
    public bool EnableInProduction { get; set; } = false;

    /// <summary>
    /// 路由前缀
    /// </summary>
    public string RoutePrefix { get; set; } = "swagger";

    /// <summary>
    /// 文档标题
    /// </summary>
    public string DocumentTitle { get; set; } = "KiteServer API 文档";
}

/// <summary>
/// 联系人信息
/// </summary>
public class ContactInfo
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = "KiteServer Team";

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = "support@kiteserver.com";

    /// <summary>
    /// 网址
    /// </summary>
    public string Url { get; set; } = "https://github.com/kiteserver";
}

/// <summary>
/// 许可证信息
/// </summary>
public class LicenseInfo
{
    /// <summary>
    /// 许可证名称
    /// </summary>
    public string Name { get; set; } = "MIT";

    /// <summary>
    /// 许可证网址
    /// </summary>
    public string Url { get; set; } = "https://opensource.org/licenses/MIT";
}

/// <summary>
/// 服务器信息
/// </summary>
public class ServerInfo
{
    /// <summary>
    /// 服务器地址
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 服务器描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
