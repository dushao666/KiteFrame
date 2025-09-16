namespace Application.Commands.Role;

/// <summary>
/// 分配角色权限命令
/// </summary>
public class AssignRolePermissionsCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<long> MenuIds { get; set; } = new();
} 