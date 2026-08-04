namespace Api.Authorization;

/// <summary>
/// 权限点授权要求：策略以权限点名命名，端点通过 [Authorize(Policy = "system:user:list")] 挂载
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permission">权限点标识（与 sys_menu.Permissions 中的权限点对应）</param>
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    /// <summary>
    /// 权限点标识
    /// </summary>
    public string Permission { get; }
}
