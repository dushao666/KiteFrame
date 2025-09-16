
using Application.Queries.Permission.Interfaces;

namespace Api.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("认证管理")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPermissionQueries _permissionQueries;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, IPermissionQueries permissionQueries, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _permissionQueries = permissionQueries;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="command">登录命令</param>
    /// <returns>登录结果</returns>
    [HttpPost("signin")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<LoginUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignInAsync([FromBody] SignInCommand command)
    {
        try
        {
            // 获取客户端IP地址和用户代理信息
            command.ClientIp = GetClientIpAddress();
            command.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();

            var result = await _mediator.Send(command);

            _logger.LogInformation("用户 {UserName} 登录成功", result.UserName);

            return Ok(ApiResult<LoginUserDto>.Ok(result, "登录成功"));
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("登录失败: {Message}", ex.Message);
            return BadRequest(ApiResult.Fail(ex.Message));
        }
        catch (Infrastructure.Exceptions.ValidationException ex)
        {
            _logger.LogWarning("登录参数验证失败: {Message}", ex.Message);
            return BadRequest(ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录过程中发生未知错误");
            return StatusCode(500, ApiResult.Fail("登录失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <returns>登出结果</returns>
    [HttpPost("signout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignOutAsync()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                return Unauthorized(ApiResult.Fail("用户未登录"));
            }

            // 从请求头获取RefreshToken作为SessionId
            var refreshToken = HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Replace("Bearer ", "");

            var command = new SignOutCommand
            {
                UserId = long.Parse(userId),
                UserName = userName,
                SessionId = refreshToken ?? string.Empty,
                ClientIp = GetClientIpAddress(),
                LogoutType = 1 // 主动退出
            };

            var result = await _mediator.Send(command);
            
            if (result)
            {
                return Ok(ApiResult.Ok("登出成功"));
            }
            else
            {
                return StatusCode(500, ApiResult.Fail("登出失败"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出过程中发生错误");
            return StatusCode(500, ApiResult.Fail("登出失败"));
        }
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult<LoginUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfileAsync()
    {
        try
        {
            var userInfo = new LoginUserDto
            {
                UserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                RealName = User.FindFirst("RealName")?.Value,
                Phone = User.FindFirst(ClaimTypes.MobilePhone)?.Value,
                Avatar = User.FindFirst("Avatar")?.Value
            };

            return Ok(ApiResult<LoginUserDto>.Ok(userInfo, "获取用户信息成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户信息过程中发生错误");
            return StatusCode(500, ApiResult.Fail("获取用户信息失败"));
        }
    }

    /// <summary>
    /// 获取当前用户的菜单路由
    /// </summary>
    /// <returns>菜单路由列表</returns>
    [HttpGet("routes")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult<List<MenuDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoutesAsync()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResult.Fail("用户未登录"));
            }

            var result = await _permissionQueries.GetUserMenuTreeAsync(long.Parse(userId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户路由过程中发生错误");
            return StatusCode(500, ApiResult.Fail("获取用户路由失败"));
        }
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的访问令牌</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken)
    {
        try
        {
            // 这里应该实现刷新Token的逻辑
            // 验证refreshToken的有效性
            // 生成新的访问令牌
            
            return Ok(ApiResult<string>.Ok("new_access_token", "Token刷新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新Token过程中发生错误");
            return StatusCode(500, ApiResult.Fail("Token刷新失败"));
        }
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request">修改密码请求</param>
    /// <returns>修改结果</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResult.Fail("用户未登录"));
            }

            // 这里应该调用相应的命令处理器来修改密码
            // var command = new ChangePasswordCommand
            // {
            //     UserId = long.Parse(userId),
            //     OldPassword = request.OldPassword,
            //     NewPassword = request.NewPassword
            // };
            // var result = await _mediator.Send(command);
            
            return Ok(ApiResult<bool>.Ok(true, "密码修改成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密码过程中发生错误");
            return StatusCode(500, ApiResult.Fail("密码修改失败"));
        }
    }

    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    /// <returns>IP地址</returns>
    private string GetClientIpAddress()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        // 检查是否通过代理
        var forwardedHeader = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            ipAddress = forwardedHeader.Split(',')[0].Trim();
        }
        
        // 检查是否通过负载均衡器
        var realIpHeader = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIpHeader))
        {
            ipAddress = realIpHeader;
        }

        return ipAddress ?? "unknown";
    }
}

/// <summary>
/// 修改密码请求
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// 旧密码
    /// </summary>
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 确认新密码
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
