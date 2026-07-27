namespace Shared.Models.Monitor;

/// <summary>
/// 获取操作日志请求
/// </summary>
public class GetOperationLogsRequest : BasePageRequest
{
    /// <summary>
    /// 用户名关键词
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 模块名称
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 操作状态（1：正常，0：异常）
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