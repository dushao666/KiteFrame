namespace Domain.Entities;

/// <summary>
/// 角色菜单权限关联实体
/// </summary>
[SugarTable("sys_role_menu")]
public class RoleMenu
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "主键ID")]
    public long Id { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnDescription = "角色ID", IsNullable = false)]
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    [SugarColumn(ColumnDescription = "菜单ID", IsNullable = false)]
    public long MenuId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsNullable = false)]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 创建用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建用户ID", IsNullable = true)]
    public long? CreateUserId { get; set; }

    /// <summary>
    /// 关联的角色（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Role? Role { get; set; }

    /// <summary>
    /// 关联的菜单（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Menu? Menu { get; set; }
}
