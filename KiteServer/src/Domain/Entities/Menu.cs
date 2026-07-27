namespace Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
[SugarTable("sys_menu")]
public class Menu : BaseEntity
{
    /// <summary>
    /// 父菜单ID（0表示根菜单）
    /// </summary>
    [SugarColumn(ColumnDescription = "父菜单ID")]
    public long ParentId { get; set; } = 0;

    /// <summary>
    /// 菜单名称
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单名称", Length = 50, IsNullable = false)]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码（唯一标识）
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单编码", Length = 100, IsNullable = false)]
    public string MenuCode { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型（1：目录，2：菜单，3：按钮）
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单类型")]
    public Shared.Enums.MenuType MenuType { get; set; } = Shared.Enums.MenuType.Menu;

    /// <summary>
    /// 路由路径
    /// </summary>
    [SugarColumn(ColumnDescription = "路由路径", Length = 200, IsNullable = true)]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [SugarColumn(ColumnDescription = "组件路径", Length = 200, IsNullable = true)]
    public string? Component { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单图标", Length = 100, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否显示（0：隐藏，1：显示）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否显示")]
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 是否缓存（0：不缓存，1：缓存）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否缓存")]
    public bool IsCache { get; set; } = false;

    /// <summary>
    /// 是否外链（0：否，1：是）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否外链")]
    public bool IsFrame { get; set; } = false;

    /// <summary>
    /// 状态（0：禁用，1：启用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 权限标识（多个用逗号分隔）
    /// </summary>
    [SugarColumn(ColumnDescription = "权限标识", Length = 500, IsNullable = true)]
    public string? Permissions { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 子菜单列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> Children { get; set; } = new List<Menu>();

    /// <summary>
    /// 父菜单（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Menu? Parent { get; set; }
}
