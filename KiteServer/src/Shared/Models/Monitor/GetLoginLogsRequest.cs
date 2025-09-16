namespace Shared.Models.Monitor;

/// <summary>
/// 获取登录日志请求
/// </summary>
public class GetLoginLogsRequest : BasePageRequest
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
    /// 登录状态（1：成功，0：失败）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
} 