using Shared.Models.User;

namespace Application.Queries.User.Interfaces;

/// <summary>
/// 用户查询服务接口
/// </summary>
public interface IUserQueries
{
    /// <summary>
    /// 分页获取用户列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>用户列表</returns>
    Task<ApiResult<PagedResult<UserDto>>> GetUsersAsync(GetUsersRequest request);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    Task<ApiResult<UserDto>> GetUserByIdAsync(long id);

    /// <summary>
    /// 获取用户已分配的角色ID列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>角色ID列表</returns>
    Task<ApiResult<List<long>>> GetUserRoleIdsAsync(long userId);
}
