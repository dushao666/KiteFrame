using Shared.Models.Permission;


namespace Application.Queries.Permission;

/// <summary>
/// 权限查询服务实现
/// </summary>
public class PermissionQueries : IPermissionQueries
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<PermissionQueries> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PermissionQueries(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<PermissionQueries> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户权限信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户权限信息</returns>
    public async Task<ApiResult<UserPermissionDto>> GetUserPermissionsAsync(long userId)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            // 获取用户角色
            var userRoles = await context.Db.Queryable<UserRole>()
                .LeftJoin<Domain.Entities.Role>((ur, r) => ur.RoleId == r.Id)
                .Where((ur, r) => ur.UserId == userId && r.Status == 1)
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
                var empty = new UserPermissionDto
                {
                    UserId = userId,
                    UserName = string.Empty,
                    Roles = new List<RoleDto>(),
                    Menus = new List<MenuDto>(),
                    Permissions = new List<string>()
                };
                return ApiResult<UserPermissionDto>.Ok(empty, "获取成功");
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

            var data = new UserPermissionDto
            {
                UserId = userId,
                UserName = string.Empty,
                Roles = userRoles,
                Menus = menuTree,
                Permissions = permissions
            };
            return ApiResult<UserPermissionDto>.Ok(data, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户权限失败，用户ID: {UserId}", userId);
            return ApiResult<UserPermissionDto>.Fail("获取用户权限失败");
        }
    }

    /// <summary>
    /// 获取用户菜单树
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>菜单树</returns>
    public async Task<ApiResult<List<MenuDto>>> GetUserMenuTreeAsync(long userId)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            // 获取用户角色ID
            var roleIds = await context.Db.Queryable<UserRole>()
                .LeftJoin<Domain.Entities.Role>((ur, r) => ur.RoleId == r.Id)
                .Where((ur, r) => ur.UserId == userId && r.Status == 1)
                .Select((ur, r) => r.Id)
                .ToListAsync();

            if (!roleIds.Any())
            {
                return ApiResult<List<MenuDto>>.Ok(new List<MenuDto>(), "获取成功");
            }

            // 获取角色对应的菜单
            var menus = await context.Db.Queryable<RoleMenu>()
                .LeftJoin<Menu>((rm, m) => rm.MenuId == m.Id)
                .Where((rm, m) => roleIds.Contains(rm.RoleId) && m.Status == 1 && m.IsVisible == true)
                .Select((rm, m) => m)
                .ToListAsync();

            // 转换为DTO并构建树结构
            var menuDtos = menus.Adapt<List<MenuDto>>();
            var tree = BuildMenuTree(menuDtos);
            return ApiResult<List<MenuDto>>.Ok(tree, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户菜单树失败，用户ID: {UserId}", userId);
            return ApiResult<List<MenuDto>>.Fail("获取用户菜单树失败");
        }
    }

    /// <summary>
    /// 检查用户权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permission">权限标识</param>
    /// <returns>是否有权限</returns>
    public async Task<ApiResult<bool>> CheckUserPermissionAsync(long userId, string permission)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            // 获取用户角色ID
            var roleIds = await context.Db.Queryable<UserRole>()
                .LeftJoin<Domain.Entities.Role>((ur, r) => ur.RoleId == r.Id)
                .Where((ur, r) => ur.UserId == userId && r.Status == 1)
                .Select((ur, r) => r.Id)
                .ToListAsync();

            if (!roleIds.Any())
            {
                return ApiResult<bool>.Ok(false);
            }

            // 检查权限
            var hasPermission = await context.Db.Queryable<RoleMenu>()
                .LeftJoin<Menu>((rm, m) => rm.MenuId == m.Id)
                .Where((rm, m) => roleIds.Contains(rm.RoleId) && m.Status == 1)
                .Where((rm, m) => !string.IsNullOrEmpty(m.Permissions) && m.Permissions.Contains(permission))
                .AnyAsync();

            return ApiResult<bool>.Ok(hasPermission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查用户权限失败，用户ID: {UserId}, 权限: {Permission}", userId, permission);
            return ApiResult<bool>.Fail("检查用户权限失败");
        }
    }

    /// <summary>
    /// 获取所有菜单列表（用于权限配置）
    /// </summary>
    /// <returns>菜单列表</returns>
    public async Task<ApiResult<List<MenuDto>>> GetAllMenusAsync()
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var menus = await context.Menus
                .AsQueryable()
                .Where(x => x.Status == 1)
                .OrderBy(x => x.Sort)
                .OrderBy(x => x.CreateTime)
                .ToListAsync();

            var menuDtos = menus.Adapt<List<MenuDto>>();
            var tree = BuildMenuTree(menuDtos);
            
            return ApiResult<List<MenuDto>>.Ok(tree, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有菜单列表失败");
            return ApiResult<List<MenuDto>>.Fail("获取菜单列表失败");
        }
    }

    /// <summary>
    /// 根据角色ID获取菜单权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单权限列表</returns>
    public async Task<ApiResult<List<long>>> GetRoleMenuIdsAsync(long roleId)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var menuIds = await context.Db.Queryable<RoleMenu>()
                .Where(rm => rm.RoleId == roleId)
                .Select(rm => rm.MenuId)
                .ToListAsync();

            return ApiResult<List<long>>.Ok(menuIds, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色菜单权限失败，角色ID: {RoleId}", roleId);
            return ApiResult<List<long>>.Fail("获取角色菜单权限失败");
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
