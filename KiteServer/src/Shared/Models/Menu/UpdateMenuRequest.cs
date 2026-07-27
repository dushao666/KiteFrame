namespace Shared.Models.Menu;

/// <summary>
/// 更新菜单请求
/// </summary>
public class UpdateMenuRequest
{
    /// <summary>
    /// 父菜单ID（0表示根菜单）
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required(ErrorMessage = "菜单名称不能为空")]
    [StringLength(100, ErrorMessage = "菜单名称长度不能超过100个字符")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码（唯一标识）
    /// </summary>
    [Required(ErrorMessage = "菜单编码不能为空")]
    [StringLength(100, ErrorMessage = "菜单编码长度不能超过100个字符")]
    public string MenuCode { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型（1：目录，2：菜单，3：按钮）
    /// </summary>
    public Shared.Enums.MenuType MenuType { get; set; } = Shared.Enums.MenuType.Menu;

    /// <summary>
    /// 路由路径
    /// </summary>
    [StringLength(200, ErrorMessage = "路由路径长度不能超过200个字符")]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [StringLength(200, ErrorMessage = "组件路径长度不能超过200个字符")]
    public string? Component { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    [StringLength(100, ErrorMessage = "菜单图标长度不能超过100个字符")]
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
    [Range(0, 1, ErrorMessage = "状态必须是0或1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 权限标识（多个用逗号分隔）
    /// </summary>
    [StringLength(500, ErrorMessage = "权限标识长度不能超过500个字符")]
    public string? Permissions { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
