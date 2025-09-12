namespace Application.Handlers.Permission;

/// <summary>
/// 获取用户权限查询处理器
/// </summary>
public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, UserPermissionDto>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetUserPermissionsQueryHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<GetUserPermissionsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理获取用户权限查询
    /// </summary>
    public async Task<UserPermissionDto> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            // 获取用户角色
            var userRoles = await context.Db.Queryable<UserRole>()
                .LeftJoin<Role>((ur, r) => ur.RoleId == r.Id)
                .Where((ur, r) => ur.UserId == request.UserId && r.Status == 1)
                .Select((ur, r) => new RoleDto
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    RoleCode = r.RoleCode,
                    Sort = r.Sort,
                    Status = r.Status,
                    DataScope = (int)r.DataScope,
                    Remark = r.Remark
                })
                .ToListAsync();

            if (!userRoles.Any())
            {
                return new UserPermissionDto
                {
                    Roles = new List<RoleDto>(),
                    Menus = new List<MenuDto>(),
                    Permissions = new List<string>()
                };
            }

            var roleIds = userRoles.Select(r => r.Id).ToList();

            // 获取角色对应的菜单
            var roleMenus = await context.Db.Queryable<RoleMenu>()
                .LeftJoin<Menu>((rm, m) => rm.MenuId == m.Id)
                .Where((rm, m) => roleIds.Contains(rm.RoleId) && m.Status == 1)
                .Select((rm, m) => m)
                .ToListAsync();

            // 构建菜单树
            var menuDtos = roleMenus.Adapt<List<MenuDto>>();
            var menuTree = BuildMenuTree(menuDtos);

            // 获取权限列表
            var permissions = roleMenus
                .Where(m => !string.IsNullOrEmpty(m.Permissions))
                .SelectMany(m => m.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();

            return new UserPermissionDto
            {
                Roles = userRoles,
                Menus = menuTree,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户权限失败，用户ID: {UserId}", request.UserId);
            throw;
        }
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    private List<MenuDto> BuildMenuTree(List<MenuDto> menus)
    {
        var menuDict = menus.ToDictionary(m => m.Id);
        var rootMenus = new List<MenuDto>();

        foreach (var menu in menus)
        {
            if (menu.ParentId == 0)
            {
                rootMenus.Add(menu);
            }
            else if (menuDict.TryGetValue(menu.ParentId, out var parent))
            {
                parent.Children ??= new List<MenuDto>();
                parent.Children.Add(menu);
            }
        }

        return rootMenus.OrderBy(m => m.Sort).ToList();
    }
}
