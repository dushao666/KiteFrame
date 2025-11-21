namespace Shared.Models.Dtos;

/// <summary>
/// 刷新Token响应DTO
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 令牌过期时间
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
