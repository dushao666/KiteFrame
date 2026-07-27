namespace Shared.Models.Role;

/// <summary>
/// 分配角色权限请求
/// </summary>
public class AssignRolePermissionsRequest
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Required(ErrorMessage = "角色ID不能为空")]
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    [Required(ErrorMessage = "菜单列表不能为空")]
    public List<long> MenuIds { get; set; } = new();
} 