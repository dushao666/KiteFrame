namespace Shared.Configuration;

/// <summary>
/// JWT 配置设置
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// 密钥
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 发行者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 受众
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public int ExpiryMinutes { get; set; } = 60;

    /// <summary>
    /// 刷新令牌过期天数
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
