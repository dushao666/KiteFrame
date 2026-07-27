namespace Shared.Configuration;

/// <summary>
/// CORS 配置设置
/// </summary>
public class CorsSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "CorsSettings";

    /// <summary>
    /// 是否启用 CORS
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 允许的源
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 是否允许凭据
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// 允许的 HTTP 方法
    /// </summary>
    public string[] AllowedMethods { get; set; } = { "GET", "POST", "PUT", "DELETE", "OPTIONS" };

    /// <summary>
    /// 允许的请求头
    /// </summary>
    public string[] AllowedHeaders { get; set; } = { "*" };

    /// <summary>
    /// 暴露的响应头
    /// </summary>
    public string[] ExposedHeaders { get; set; } = Array.Empty<string>();
}
