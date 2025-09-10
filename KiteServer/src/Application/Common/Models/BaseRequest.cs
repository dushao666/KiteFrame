namespace Application.Common.Models;

/// <summary>
/// 基础请求模型
/// </summary>
public abstract class BaseRequest
{
}

/// <summary>
/// 分页请求模型
/// </summary>
public class PagedRequest : BaseRequest
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    public string? SortOrder { get; set; } = "desc";
}
