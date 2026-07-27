using Application.Queries.Monitor.Interfaces;
using Shared.Models.Monitor;
using Shared.Models.Dtos;
using Domain.Entities;

namespace Application.Queries.Monitor;

/// <summary>
/// 系统监控查询服务实现
/// </summary>
public class MonitorQueries : IMonitorQueries
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<MonitorQueries> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MonitorQueries(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<MonitorQueries> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    public async Task<ApiResult<PagedResult<OnlineUserDto>>> GetOnlineUsersAsync(GetOnlineUsersRequest request)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var query = context.Db.Queryable<OnlineUser>()
                .Where(u => u.Status == 1) // 只查询在线用户
                .WhereIF(!string.IsNullOrEmpty(request.UserName), u => u.UserName.Contains(request.UserName!))
                .WhereIF(!string.IsNullOrEmpty(request.IpAddress), u => u.IpAddress!.Contains(request.IpAddress!))
                .WhereIF(request.Status.HasValue, u => u.Status == request.Status!.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(u => u.LoginTime)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = items.Adapt<List<OnlineUserDto>>();
            var result = new PagedResult<OnlineUserDto>
            {
                Items = dtos,
                TotalCount = total,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return ApiResult<PagedResult<OnlineUserDto>>.Ok(result, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取在线用户列表失败");
            return ApiResult<PagedResult<OnlineUserDto>>.Fail("获取在线用户列表失败");
        }
    }

    /// <summary>
    /// 根据SessionId获取在线用户
    /// </summary>
    public async Task<ApiResult<OnlineUserDto?>> GetOnlineUserBySessionIdAsync(string sessionId)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var onlineUser = await context.Db.Queryable<OnlineUser>()
                .Where(u => u.SessionId == sessionId)
                .FirstAsync();

            var dto = onlineUser?.Adapt<OnlineUserDto>();
            return ApiResult<OnlineUserDto?>.Ok(dto, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取在线用户失败，SessionId: {SessionId}", sessionId);
            return ApiResult<OnlineUserDto?>.Fail("获取在线用户失败");
        }
    }

    /// <summary>
    /// 强制用户下线
    /// </summary>
    public async Task<ApiResult<bool>> ForceLogoutAsync(string sessionId)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            var updated = await context.Db.Updateable<OnlineUser>()
                .SetColumns(u => new OnlineUser { Status = 0, UpdateTime = DateTime.Now })
                .Where(u => u.SessionId == sessionId)
                .ExecuteCommandAsync();

            context.Commit();

            if (updated > 0)
            {
                _logger.LogInformation("用户强制下线成功，SessionId: {SessionId}", sessionId);
                return ApiResult<bool>.Ok(true, "强制下线成功");
            }
            else
            {
                return ApiResult<bool>.Fail("用户不存在或已下线");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "强制用户下线失败，SessionId: {SessionId}", sessionId);
            return ApiResult<bool>.Fail("强制下线失败");
        }
    }

    /// <summary>
    /// 获取登录日志列表
    /// </summary>
    public async Task<ApiResult<PagedResult<LoginLogDto>>> GetLoginLogsAsync(GetLoginLogsRequest request)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var query = context.Db.Queryable<LoginLog>()
                .WhereIF(!string.IsNullOrEmpty(request.UserName), l => l.UserName!.Contains(request.UserName!))
                .WhereIF(!string.IsNullOrEmpty(request.IpAddress), l => l.IpAddress!.Contains(request.IpAddress!))
                .WhereIF(request.Status.HasValue, l => l.Status == request.Status!.Value)
                .WhereIF(request.StartTime.HasValue, l => l.LoginTime >= request.StartTime!.Value)
                .WhereIF(request.EndTime.HasValue, l => l.LoginTime <= request.EndTime!.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.LoginTime)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = items.Adapt<List<LoginLogDto>>();
            var result = new PagedResult<LoginLogDto>
            {
                Items = dtos,
                TotalCount = total,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return ApiResult<PagedResult<LoginLogDto>>.Ok(result, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取登录日志列表失败");
            return ApiResult<PagedResult<LoginLogDto>>.Fail("获取登录日志列表失败");
        }
    }

    /// <summary>
    /// 删除登录日志
    /// </summary>
    public async Task<ApiResult<bool>> DeleteLoginLogsAsync(List<long> ids)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            var deleted = await context.Db.Deleteable<LoginLog>()
                .Where(l => ids.Contains(l.Id))
                .ExecuteCommandAsync();

            context.Commit();

            _logger.LogInformation("删除登录日志成功，删除数量: {Count}", deleted);
            return ApiResult<bool>.Ok(true, $"删除成功，共删除 {deleted} 条记录");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除登录日志失败");
            return ApiResult<bool>.Fail("删除登录日志失败");
        }
    }

    /// <summary>
    /// 清空登录日志
    /// </summary>
    public async Task<ApiResult<bool>> ClearLoginLogsAsync()
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            var deleted = await context.Db.Deleteable<LoginLog>()
                .Where(l => true) // 删除所有记录
                .ExecuteCommandAsync();

            context.Commit();

            _logger.LogInformation("清空登录日志成功，删除数量: {Count}", deleted);
            return ApiResult<bool>.Ok(true, $"清空成功，共删除 {deleted} 条记录");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清空登录日志失败");
            return ApiResult<bool>.Fail("清空登录日志失败");
        }
    }

    /// <summary>
    /// 获取操作日志列表
    /// </summary>
    public async Task<ApiResult<PagedResult<OperationLogDto>>> GetOperationLogsAsync(GetOperationLogsRequest request)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(false);

            var query = context.Db.Queryable<OperationLog>()
                .WhereIF(!string.IsNullOrEmpty(request.UserName), l => l.UserName!.Contains(request.UserName!))
                .WhereIF(!string.IsNullOrEmpty(request.Module), l => l.Module!.Contains(request.Module!))
                .WhereIF(!string.IsNullOrEmpty(request.BusinessType), l => l.BusinessType!.Contains(request.BusinessType!))
                .WhereIF(request.Status.HasValue, l => l.Status == request.Status!.Value)
                .WhereIF(request.StartTime.HasValue, l => l.OperTime >= request.StartTime!.Value)
                .WhereIF(request.EndTime.HasValue, l => l.OperTime <= request.EndTime!.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.OperTime)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = items.Adapt<List<OperationLogDto>>();
            var result = new PagedResult<OperationLogDto>
            {
                Items = dtos,
                TotalCount = total,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return ApiResult<PagedResult<OperationLogDto>>.Ok(result, "获取成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取操作日志列表失败");
            return ApiResult<PagedResult<OperationLogDto>>.Fail("获取操作日志列表失败");
        }
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    public async Task<ApiResult<bool>> DeleteOperationLogsAsync(List<long> ids)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            var deleted = await context.Db.Deleteable<OperationLog>()
                .Where(l => ids.Contains(l.Id))
                .ExecuteCommandAsync();

            context.Commit();

            _logger.LogInformation("删除操作日志成功，删除数量: {Count}", deleted);
            return ApiResult<bool>.Ok(true, $"删除成功，共删除 {deleted} 条记录");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除操作日志失败");
            return ApiResult<bool>.Fail("删除操作日志失败");
        }
    }

    /// <summary>
    /// 清空操作日志
    /// </summary>
    public async Task<ApiResult<bool>> ClearOperationLogsAsync()
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            var deleted = await context.Db.Deleteable<OperationLog>()
                .Where(l => true) // 删除所有记录
                .ExecuteCommandAsync();

            context.Commit();

            _logger.LogInformation("清空操作日志成功，删除数量: {Count}", deleted);
            return ApiResult<bool>.Ok(true, $"清空成功，共删除 {deleted} 条记录");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清空操作日志失败");
            return ApiResult<bool>.Fail("清空操作日志失败");
        }
    }
} 