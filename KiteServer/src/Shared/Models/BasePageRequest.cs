namespace Shared.Models;

/// <summary>
/// 分页请求基类
/// </summary>
public class BasePageRequest
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
} 