namespace Application.Commands.Auth;

/// <summary>
/// 用户退出登录命令
/// </summary>
public class SignOutCommand : IRequest<bool>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 会话ID（RefreshToken）
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 退出类型（1：主动退出，2：强制下线，3：Token过期）
    /// </summary>
    public int LogoutType { get; set; } = 1;
}
