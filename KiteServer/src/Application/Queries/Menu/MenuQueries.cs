using Shared.Models.Menu;

namespace Application.Queries.Menu;

/// <summary>
/// 菜单查询服务
/// </summary>
public class MenuQueries : IMenuQueries
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<MenuQueries> _logger;

    public MenuQueries(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<MenuQueries> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 获取菜单列表（分页）
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>菜单列表</returns>
    public async Task<ApiResult<PagedResult<MenuDto>>> GetMenusAsync(GetMenusRequest request)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var query = context.Menus
                    .AsQueryable()
                    .Where(x => !x.IsDeleted);

                // 菜单名称模糊查询
                if (!string.IsNullOrWhiteSpace(request.MenuName))
                {
                    query = query.Where(x => x.MenuName.Contains(request.MenuName));
                }

                // 菜单编码模糊查询
                if (!string.IsNullOrWhiteSpace(request.MenuCode))
                {
                    query = query.Where(x => x.MenuCode.Contains(request.MenuCode));
                }

                // 菜单类型筛选
                if (request.MenuType.HasValue)
                {
                    query = query.Where(x => x.MenuType == request.MenuType.Value);
                }

                // 状态筛选
                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.Value);
                }

                // 是否显示筛选
                if (request.IsVisible.HasValue)
                {
                    query = query.Where(x => x.IsVisible == request.IsVisible.Value);
                }

                // 排序
                query = query.OrderBy(x => x.Sort).OrderBy(x => x.CreateTime);

                // 分页查询
                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var menuDtos = items.Adapt<List<MenuDto>>();

                var result = new PagedResult<MenuDto>
                {
                    Items = menuDtos,
                    TotalCount = totalCount,
                    PageIndex = request.PageNumber,
                    PageSize = request.PageSize
                };

                return ApiResult<PagedResult<MenuDto>>.Ok(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单列表失败");
            return ApiResult<PagedResult<MenuDto>>.Fail("获取菜单列表失败");
        }
    }

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单信息</returns>
    public async Task<ApiResult<MenuDto>> GetMenuByIdAsync(long id)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var menu = await context.Menus
                    .AsQueryable()
                    .Where(x => x.Id == id && !x.IsDeleted)
                    .FirstAsync();

                if (menu == null)
                {
                    return ApiResult<MenuDto>.Fail("菜单不存在");
                }

                var menuDto = menu.Adapt<MenuDto>();
                return ApiResult<MenuDto>.Ok(menuDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单详情失败，菜单ID: {MenuId}", id);
            return ApiResult<MenuDto>.Fail("获取菜单详情失败");
        }
    }

    /// <summary>
    /// 获取菜单树形结构
    /// </summary>
    /// <returns>菜单树</returns>
    public async Task<ApiResult<List<MenuDto>>> GetMenuTreeAsync()
    {
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var allMenus = await context.Menus
                    .AsQueryable()
                    .Where(x => !x.IsDeleted && x.Status == 1)
                    .OrderBy(x => x.Sort)
                    .OrderBy(x => x.CreateTime)
                    .ToListAsync();

                var menuDtos = allMenus.Adapt<List<MenuDto>>();
                var menuTree = BuildMenuTree(menuDtos, 0);

                return ApiResult<List<MenuDto>>.Ok(menuTree);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单树失败");
            return ApiResult<List<MenuDto>>.Fail("获取菜单树失败");
        }
    }

    /// <summary>
    /// 根据用户ID获取用户菜单权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户菜单列表</returns>
    public async Task<ApiResult<List<MenuDto>>> GetUserMenusAsync(long userId)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 获取用户的角色菜单权限
                var userMenus = await context.Users
                    .AsQueryable()
                    .LeftJoin<Domain.Entities.UserRole>((u, ur) => u.Id == ur.UserId)
                    .LeftJoin<Domain.Entities.RoleMenu>((u, ur, rm) => ur.RoleId == rm.RoleId)
                    .LeftJoin<Domain.Entities.Menu>((u, ur, rm, m) => rm.MenuId == m.Id)
                    .Where((u, ur, rm, m) => u.Id == userId && !u.IsDeleted && !m.IsDeleted && m.Status == 1)
                    .Select((u, ur, rm, m) => m)
                    .Distinct()
                    .OrderBy(m => m.Sort)
                    .OrderBy(m => m.CreateTime)
                    .ToListAsync();

                var menuDtos = userMenus.Adapt<List<MenuDto>>();
                var menuTree = BuildMenuTree(menuDtos, 0);

                return ApiResult<List<MenuDto>>.Ok(menuTree);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户菜单失败，用户ID: {UserId}", userId);
            return ApiResult<List<MenuDto>>.Fail("获取用户菜单失败");
        }
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    /// <param name="menus">菜单列表</param>
    /// <param name="parentId">父菜单ID</param>
    /// <returns>菜单树</returns>
    private List<MenuDto> BuildMenuTree(List<MenuDto> menus, long parentId)
    {
        var result = new List<MenuDto>();

        var children = menus.Where(x => x.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            child.Children = BuildMenuTree(menus, child.Id);
            result.Add(child);
        }

        return result;
    }
}
