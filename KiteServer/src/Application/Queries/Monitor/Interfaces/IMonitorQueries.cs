using Shared.Models.Monitor;
using Shared.Models.Dtos;

namespace Application.Queries.Monitor.Interfaces;

/// <summary>
/// 系统监控查询服务接口
/// </summary>
public interface IMonitorQueries
{
    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>在线用户列表</returns>
    Task<ApiResult<PagedResult<OnlineUserDto>>> GetOnlineUsersAsync(GetOnlineUsersRequest request);

    /// <summary>
    /// 根据SessionId获取在线用户
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <returns>在线用户信息</returns>
    Task<ApiResult<OnlineUserDto?>> GetOnlineUserBySessionIdAsync(string sessionId);

    /// <summary>
    /// 强制用户下线
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <returns>操作结果</returns>
    Task<ApiResult<bool>> ForceLogoutAsync(string sessionId);

    /// <summary>
    /// 获取登录日志列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>登录日志列表</returns>
    Task<ApiResult<PagedResult<LoginLogDto>>> GetLoginLogsAsync(GetLoginLogsRequest request);

    /// <summary>
    /// 删除登录日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>操作结果</returns>
    Task<ApiResult<bool>> DeleteLoginLogsAsync(List<long> ids);

    /// <summary>
    /// 清空登录日志
    /// </summary>
    /// <returns>操作结果</returns>
    Task<ApiResult<bool>> ClearLoginLogsAsync();

    /// <summary>
    /// 获取操作日志列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>操作日志列表</returns>
    Task<ApiResult<PagedResult<OperationLogDto>>> GetOperationLogsAsync(GetOperationLogsRequest request);

    /// <summary>
    /// 删除操作日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>操作结果</returns>
    Task<ApiResult<bool>> DeleteOperationLogsAsync(List<long> ids);

    /// <summary>
    /// 清空操作日志
    /// </summary>
    /// <returns>操作结果</returns>
    Task<ApiResult<bool>> ClearOperationLogsAsync();
} 