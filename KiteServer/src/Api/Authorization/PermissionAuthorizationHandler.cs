namespace Api.Authorization;

/// <summary>
/// 权限点授权处理器：根据 RBAC（用户-角色-菜单权限点）判定当前用户是否拥有所需权限点
/// 以 Scoped 生命周期注册，便于注入应用层查询服务
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionQueries _permissionQueries;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissionQueries">权限查询服务</param>
    /// <param name="logger">日志</param>
    public PermissionAuthorizationHandler(IPermissionQueries permissionQueries, ILogger<PermissionAuthorizationHandler> logger)
    {
        _permissionQueries = permissionQueries;
        _logger = logger;
    }

    /// <summary>
    /// 处理权限点授权判定
    /// </summary>
    /// <param name="context">授权上下文</param>
    /// <param name="requirement">权限点要求</param>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        try
        {
            var result = await _permissionQueries.GetUserPermissionsAsync(userId);
            if (result is { Success: true, Data: not null }
                && result.Data.Permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("用户 {UserId} 缺少权限点 {Permission}，访问被拒绝", userId, requirement.Permission);
            }
        }
        catch (Exception ex)
        {
            // 权限查询失败按拒绝处理，不向调用方抛出异常
            _logger.LogError(ex, "权限点校验失败，用户 {UserId}，权限点 {Permission}", userId, requirement.Permission);
        }
    }
}
