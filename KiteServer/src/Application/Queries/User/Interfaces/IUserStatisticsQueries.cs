using Shared.Models.User;

namespace Application.Queries.User.Interfaces;

/// <summary>
/// 用户统计查询服务接口
/// </summary>
public interface IUserStatisticsQueries
{
    /// <summary>
    /// 获取用户统计信息
    /// </summary>
    /// <returns>用户统计信息</returns>
    Task<ApiResult<UserStatisticsDto>> GetUserStatisticsAsync();

    /// <summary>
    /// 获取用户注册趋势
    /// </summary>
    /// <param name="days">天数</param>
    /// <returns>注册趋势数据</returns>
    Task<ApiResult<List<UserRegistrationTrendDto>>> GetUserRegistrationTrendAsync(int days = 30);
}
