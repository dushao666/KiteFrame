namespace Shared.Models.Role;

/// <summary>
/// 获取角色列表请求
/// </summary>
public class GetRolesRequest
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
    /// 关键词（角色名称或角色编码）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态筛选（0：禁用，1：启用）
    /// </summary>
    public int? Status { get; set; }
}
