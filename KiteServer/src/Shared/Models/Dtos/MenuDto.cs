namespace Shared.Models.Dtos;

/// <summary>
/// 菜单DTO
/// </summary>
public class MenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    public string MenuCode { get; set; } = string.Empty;

    /// <summary>
    /// 菜单类型（1：目录，2：菜单，3：按钮）
    /// </summary>
    public int MenuType { get; set; }

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
    /// 是否显示
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsCache { get; set; }

    /// <summary>
    /// 是否外链
    /// </summary>
    public bool IsFrame { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? Permissions { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 子菜单列表
    /// </summary>
    public List<MenuDto> Children { get; set; } = new List<MenuDto>();
}

/// <summary>
/// 角色DTO
/// </summary>
public class RoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 数据权限范围
    /// </summary>
    public int DataScope { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 用户权限信息DTO
/// </summary>
public class UserPermissionDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户角色列表
    /// </summary>
    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();

    /// <summary>
    /// 用户菜单权限（树形结构）
    /// </summary>
    public List<MenuDto> Menus { get; set; } = new List<MenuDto>();

    /// <summary>
    /// 用户按钮权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = new List<string>();
}
