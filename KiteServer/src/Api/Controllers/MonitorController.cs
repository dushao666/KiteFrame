using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// 系统监控控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonitorController : ControllerBase
{
    private readonly IMonitorQueries _monitorQueries;
    private readonly ILogger<MonitorController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MonitorController(
        IMonitorQueries monitorQueries,
        ILogger<MonitorController> logger)
    {
        _monitorQueries = monitorQueries;
        _logger = logger;
    }

    /// <summary>
    /// 获取在线用户列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>在线用户列表</returns>
    [HttpGet("online/users")]
    [ProducesResponseType(typeof(ApiResult<PagedResult<OnlineUserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOnlineUsers([FromQuery] GetOnlineUsersRequest request)
    {
        var result = await _monitorQueries.GetOnlineUsersAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// 强制用户下线
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("online/logout/{sessionId}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForceLogout(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return BadRequest(ApiResult.Fail("会话ID不能为空"));
        }

        var result = await _monitorQueries.ForceLogoutAsync(sessionId);
        return Ok(result);
    }

    /// <summary>
    /// 获取登录日志列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>登录日志列表</returns>
    [HttpGet("loginlogs")]
    [ProducesResponseType(typeof(ApiResult<PagedResult<LoginLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLoginLogs([FromQuery] GetLoginLogsRequest request)
    {
        var result = await _monitorQueries.GetLoginLogsAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// 删除登录日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>操作结果</returns>
    [HttpDelete("loginlogs")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteLoginLogs([FromBody] List<long> ids)
    {
        if (ids == null || !ids.Any())
        {
            return BadRequest(ApiResult.Fail("请选择要删除的日志"));
        }

        var result = await _monitorQueries.DeleteLoginLogsAsync(ids);
        return Ok(result);
    }

    /// <summary>
    /// 清空登录日志
    /// </summary>
    /// <returns>操作结果</returns>
    [HttpDelete("loginlogs/clear")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClearLoginLogs()
    {
        var result = await _monitorQueries.ClearLoginLogsAsync();
        return Ok(result);
    }

    /// <summary>
    /// 获取操作日志列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>操作日志列表</returns>
    [HttpGet("operationlogs")]
    [ProducesResponseType(typeof(ApiResult<PagedResult<OperationLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOperationLogs([FromQuery] GetOperationLogsRequest request)
    {
        var result = await _monitorQueries.GetOperationLogsAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    /// <param name="ids">日志ID列表</param>
    /// <returns>操作结果</returns>
    [HttpDelete("operationlogs")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOperationLogs([FromBody] List<long> ids)
    {
        if (ids == null || !ids.Any())
        {
            return BadRequest(ApiResult.Fail("请选择要删除的日志"));
        }

        var result = await _monitorQueries.DeleteOperationLogsAsync(ids);
        return Ok(result);
    }

    /// <summary>
    /// 清空操作日志
    /// </summary>
    /// <returns>操作结果</returns>
    [HttpDelete("operationlogs/clear")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClearOperationLogs()
    {
        var result = await _monitorQueries.ClearOperationLogsAsync();
        return Ok(result);
    }
} 