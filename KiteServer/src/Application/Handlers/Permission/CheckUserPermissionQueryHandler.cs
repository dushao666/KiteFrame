namespace Application.Handlers.Permission;

/// <summary>
/// 检查用户权限查询处理器
/// </summary>
public class CheckUserPermissionQueryHandler : IRequestHandler<CheckUserPermissionQuery, bool>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<CheckUserPermissionQueryHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public CheckUserPermissionQueryHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<CheckUserPermissionQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理检查用户权限查询
    /// </summary>
    public async Task<bool> Handle(CheckUserPermissionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            // 获取用户角色ID
            var roleIds = await context.Db.Queryable<UserRole>()
                .LeftJoin<Role>((ur, r) => ur.RoleId == r.Id)
                .Where((ur, r) => ur.UserId == request.UserId && r.Status == 1)
                .Select((ur, r) => r.Id)
                .ToListAsync();

            if (!roleIds.Any())
            {
                return false;
            }

            // 检查权限
            var hasPermission = await context.Db.Queryable<RoleMenu>()
                .LeftJoin<Menu>((rm, m) => rm.MenuId == m.Id)
                .Where((rm, m) => roleIds.Contains(rm.RoleId) && m.Status == 1)
                .Where((rm, m) => !string.IsNullOrEmpty(m.Permissions) && m.Permissions.Contains(request.Permission))
                .AnyAsync();

            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查用户权限失败，用户ID: {UserId}, 权限: {Permission}", request.UserId, request.Permission);
            return false;
        }
    }
}
