namespace Shared.Models.User;

/// <summary>
/// 获取用户列表请求
/// </summary>
public class GetUsersRequest
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 关键词
    /// </summary>
    public string? Keyword { get; set; }
}
