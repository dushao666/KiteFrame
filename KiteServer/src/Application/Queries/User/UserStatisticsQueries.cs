using Shared.Models.User;
namespace Application.Queries.User;

/// <summary>
/// 用户统计查询服务实现
/// </summary>
public class UserStatisticsQueries : IUserStatisticsQueries
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UserStatisticsQueries> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元</param>
    /// <param name="logger">日志记录器</param>
    public UserStatisticsQueries(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<UserStatisticsQueries> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户统计信息
    /// </summary>
    /// <returns>用户统计信息</returns>
    public async Task<ApiResult<UserStatisticsDto>> GetUserStatisticsAsync()
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var totalUsers = await context.Users.AsQueryable().CountAsync();
            var activeUsers = await context.Users.AsQueryable().CountAsync(x => x.Status == 1);
            var inactiveUsers = await context.Users.AsQueryable().CountAsync(x => x.Status == 0);
            var todayRegistrations = await context.Users.AsQueryable().CountAsync(x => x.CreateTime.Date == DateTime.Today);
            
            var statistics = new UserStatisticsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                TodayRegistrations = todayRegistrations
            };
            
            return ApiResult<UserStatisticsDto>.Ok(statistics, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户统计信息失败");
            return ApiResult<UserStatisticsDto>.Fail("获取用户统计信息失败");
        }
    }

    /// <summary>
    /// 获取用户注册趋势
    /// </summary>
    /// <param name="days">天数</param>
    /// <returns>注册趋势数据</returns>
    public async Task<ApiResult<List<UserRegistrationTrendDto>>> GetUserRegistrationTrendAsync(int days = 30)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var startDate = DateTime.Today.AddDays(-days);

            // 先获取原始数据，然后在内存中进行分组
            var users = await context.Users.AsQueryable()
                .Where(x => x.CreateTime >= startDate)
                .Select(x => new { x.CreateTime })
                .ToListAsync();

            var registrationData = users
                .GroupBy(x => x.CreateTime.Date)
                .Select(g => new UserRegistrationTrendDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();
            
            return ApiResult<List<UserRegistrationTrendDto>>.Ok(registrationData, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户注册趋势失败");
            return ApiResult<List<UserRegistrationTrendDto>>.Fail("获取用户注册趋势失败");
        }
    }
}
