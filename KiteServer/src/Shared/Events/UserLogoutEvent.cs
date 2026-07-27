using MediatR;

namespace Shared.Events;

/// <summary>
/// 用户退出登录事件
/// </summary>
public class UserLogoutEvent : INotification
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
    /// 会话ID
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 退出类型（1：主动退出，2：强制下线，3：Token过期）
    /// </summary>
    public int LogoutType { get; set; } = 1;

    /// <summary>
    /// 退出时间
    /// </summary>
    public DateTime LogoutTime { get; set; } = DateTime.Now;
}
