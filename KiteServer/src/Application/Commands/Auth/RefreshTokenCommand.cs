namespace Application.Commands.Auth;

/// <summary>
/// 刷新Token命令
/// </summary>
public class RefreshTokenCommand : IRequest<RefreshTokenDto>
{
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 用户代理信息
    /// </summary>
    public string? UserAgent { get; set; }
}
