using Shared.Models.Dtos;

namespace Shared.Models.Permission;

/// <summary>
/// 用户权限DTO
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
    /// 用户菜单列表
    /// </summary>
    public List<MenuDto> Menus { get; set; } = new List<MenuDto>();

    /// <summary>
    /// 用户权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = new List<string>();
} 