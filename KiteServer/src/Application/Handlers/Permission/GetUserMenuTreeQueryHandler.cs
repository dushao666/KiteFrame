namespace Application.Handlers.Permission;

/// <summary>
/// 获取用户菜单树查询处理器
/// </summary>
public class GetUserMenuTreeQueryHandler : IRequestHandler<GetUserMenuTreeQuery, List<MenuDto>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<GetUserMenuTreeQueryHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetUserMenuTreeQueryHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<GetUserMenuTreeQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理获取用户菜单树查询
    /// </summary>
    public async Task<List<MenuDto>> Handle(GetUserMenuTreeQuery request, CancellationToken cancellationToken)
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
                return new List<MenuDto>();
            }

            // 获取角色对应的菜单
            var menus = await context.Db.Queryable<RoleMenu>()
                .LeftJoin<Menu>((rm, m) => rm.MenuId == m.Id)
                .Where((rm, m) => roleIds.Contains(rm.RoleId) && m.Status == 1 && m.IsVisible == true)
                .Select((rm, m) => m)
                .ToListAsync();

            // 转换为DTO并构建树结构
            var menuDtos = menus.Adapt<List<MenuDto>>();
            return BuildMenuTree(menuDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户菜单树失败，用户ID: {UserId}", request.UserId);
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
