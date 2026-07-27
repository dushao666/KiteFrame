using Shared.Models.Role;

namespace Application.Queries.Role;

/// <summary>
/// 角色查询服务实现
/// </summary>
public class RoleQueries : IRoleQueries
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<RoleQueries> _logger;

    public RoleQueries(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<RoleQueries> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 分页获取角色列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>角色列表</returns>
    public async Task<ApiResult<PagedResult<RoleDto>>> GetRolesAsync(GetRolesRequest request)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                var query = context.Roles.AsQueryable();
                
                // 关键词搜索
                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    query = query.Where(x => x.RoleName.Contains(request.Keyword) || 
                                            x.RoleCode.Contains(request.Keyword));
                }

                // 状态筛选
                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.Value);
                }

                var totalCount = await query.CountAsync();
                var roles = await query.OrderBy(x => x.Sort)
                    .OrderByDescending(x => x.CreateTime)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var roleDtos = roles.Adapt<List<RoleDto>>();

                var result = new PagedResult<RoleDto>
                {
                    Items = roleDtos,
                    TotalCount = totalCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };

                return ApiResult<PagedResult<RoleDto>>.Ok(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色列表失败");
            return ApiResult<PagedResult<RoleDto>>.Fail("获取角色列表失败");
        }
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色信息</returns>
    public async Task<ApiResult<RoleDto>> GetRoleByIdAsync(long id)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                var role = await context.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResult<RoleDto>.Fail("角色不存在");
                }

                var roleDto = role.Adapt<RoleDto>();
                return ApiResult<RoleDto>.Ok(roleDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色详情失败，角色ID: {RoleId}", id);
            return ApiResult<RoleDto>.Fail("获取角色详情失败");
        }
    }

    /// <summary>
    /// 获取所有启用的角色列表（用于下拉选择）
    /// </summary>
    /// <returns>角色列表</returns>
    public async Task<ApiResult<List<RoleDto>>> GetEnabledRolesAsync()
    {
        try
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                var roles = await context.Roles
                    .AsQueryable()
                    .Where(x => x.Status == 1)
                    .OrderBy(x => x.Sort)
                    .OrderBy(x => x.RoleName)
                    .ToListAsync();

                var roleDtos = roles.Adapt<List<RoleDto>>();
                return ApiResult<List<RoleDto>>.Ok(roleDtos);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取启用角色列表失败");
            return ApiResult<List<RoleDto>>.Fail("获取启用角色列表失败");
        }
    }

    /// <summary>
    /// 检查角色编码是否存在
    /// </summary>
    /// <param name="roleCode">角色编码</param>
    /// <param name="excludeId">排除的角色ID（用于更新时检查）</param>
    /// <returns>是否存在</returns>
    public async Task<ApiResult<bool>> CheckRoleCodeExistsAsync(string roleCode, long? excludeId = null)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                var query = context.Roles.AsQueryable().Where(x => x.RoleCode == roleCode);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.Id != excludeId.Value);
                }

                var exists = await query.AnyAsync();
                return ApiResult<bool>.Ok(exists);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查角色编码是否存在失败，角色编码: {RoleCode}", roleCode);
            return ApiResult<bool>.Fail("检查角色编码失败");
        }
    }
}
