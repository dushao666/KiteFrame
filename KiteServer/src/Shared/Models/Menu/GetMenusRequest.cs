namespace Shared.Models.Menu;

/// <summary>
/// 获取菜单列表请求
/// </summary>
public class GetMenusRequest
{
    /// <summary>
    /// 菜单名称（模糊查询）
    /// </summary>
    public string? MenuName { get; set; }

    /// <summary>
    /// 菜单编码（模糊查询）
    /// </summary>
    public string? MenuCode { get; set; }

    /// <summary>
    /// 菜单类型（1：目录，2：菜单，3：按钮）
    /// </summary>
    public Shared.Enums.MenuType? MenuType { get; set; }

    /// <summary>
    /// 状态（0：禁用，1：启用）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 是否显示（0：隐藏，1：显示）
    /// </summary>
    public bool? IsVisible { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 20;
}
