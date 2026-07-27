namespace Application.Commands.Menu;

/// <summary>
/// 更新菜单命令
/// </summary>
public class UpdateMenuCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 父菜单ID（0表示根菜单）
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码（唯一标识）
    /// </summary>
    public string MenuCode { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型（1：目录，2：菜单，3：按钮）
    /// </summary>
    public Shared.Enums.MenuType MenuType { get; set; } = Shared.Enums.MenuType.Menu;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否显示（0：隐藏，1：显示）
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 是否缓存（0：不缓存，1：缓存）
    /// </summary>
    public bool IsCache { get; set; }

    /// <summary>
    /// 是否外链（0：否，1：是）
    /// </summary>
    public bool IsFrame { get; set; }

    /// <summary>
    /// 状态（0：禁用，1：启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 权限标识（多个用逗号分隔）
    /// </summary>
    public string? Permissions { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
