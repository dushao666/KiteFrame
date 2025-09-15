namespace Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
[SugarTable("sys_role")]
public class Role : BaseEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(ColumnDescription = "角色名称", Length = 50, IsNullable = false)]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码（唯一标识）
    /// </summary>
    [SugarColumn(ColumnDescription = "角色编码", Length = 100, IsNullable = false)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态（0：禁用，1：启用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 数据权限范围
    /// </summary>
    [SugarColumn(ColumnDescription = "数据权限范围")]
    public DataScope DataScope { get; set; } = DataScope.All;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 角色关联的用户列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// 角色关联的菜单列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> Menus { get; set; } = new List<Menu>();
}
