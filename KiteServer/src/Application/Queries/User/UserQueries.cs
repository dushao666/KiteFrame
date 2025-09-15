using Shared.Models.User;


namespace Application.Queries.User;

/// <summary>
/// 用户查询服务实现
/// </summary>
public class UserQueries : IUserQueries
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UserQueries> _logger;

    public UserQueries(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<UserQueries> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 分页获取用户列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>用户列表</returns>
    public async Task<ApiResult<PagedResult<UserDto>>> GetUsersAsync(GetUsersRequest request)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                var query = context.Users.AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    query = query.Where(x => x.UserName.Contains(request.Keyword) || 
                                            (x.RealName != null && x.RealName.Contains(request.Keyword)) || 
                                            (x.Email != null && x.Email.Contains(request.Keyword)));
                }

                var totalCount = await query.CountAsync();
                var users = await query.OrderByDescending(x => x.CreateTime)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                // 使用Mapster进行对象映射
                var items = users.Adapt<List<UserDto>>();
                
                var result = new PagedResult<UserDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };

                return ApiResult<PagedResult<UserDto>>.Ok(result, "获取成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户列表失败");
            return ApiResult<PagedResult<UserDto>>.Fail("获取用户列表失败");
        }
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    public async Task<ApiResult<UserDto>> GetUserByIdAsync(long id)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                var user = await context.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResult<UserDto>.Fail("用户不存在");
                }

                // 使用Mapster进行对象映射
                var userDto = user.Adapt<UserDto>();

                return ApiResult<UserDto>.Ok(userDto, "获取成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户失败，用户ID: {UserId}", id);
            return ApiResult<UserDto>.Fail("获取用户失败");
        }
    }
}
