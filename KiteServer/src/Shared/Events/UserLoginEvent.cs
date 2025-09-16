using MediatR;

namespace Shared.Events;

/// <summary>
/// 用户登录事件
/// </summary>
public class UserLoginEvent : INotification
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
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// IP归属地
    /// </summary>
    public string? IpLocation { get; set; }

    /// <summary>
    /// 浏览器类型
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    public string? Os { get; set; }

    /// <summary>
    /// 登录类型
    /// </summary>
    public LoginType LoginType { get; set; }

    /// <summary>
    /// 登录状态（1：成功，0：失败）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 登录消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Token过期时间
    /// </summary>
    public DateTime ExpireTime { get; set; }
}
