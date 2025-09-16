namespace Shared.Models.Monitor;

/// <summary>
/// 获取在线用户请求
/// </summary>
public class GetOnlineUsersRequest : BasePageRequest
{
    /// <summary>
    /// 用户名关键词
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 状态（1：在线，0：离线）
    /// </summary>
    public int? Status { get; set; }
} 