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

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mediator">MediatR</param>
    /// <param name="permissionQueries">权限查询服务</param>
    /// <param name="logger">日志</param>
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
        // 获取客户端IP地址和用户代理信息
        command.ClientIp = GetClientIpAddress();
        command.UserAgent = HttpContext.Request.Headers.UserAgent.FirstOrDefault();

        var result = await _mediator.Send(command);

        _logger.LogInformation("用户 {UserName} 登录成功", result.UserName);

        return Ok(ApiResult<LoginUserDto>.Ok(result, "登录成功"));
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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
        {
            return Unauthorized(ApiResult.Fail("用户未登录"));
        }

        // 从请求头获取RefreshToken作为SessionId
        var refreshToken = HttpContext.Request.Headers.Authorization
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

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult<LoginUserDto>), StatusCodes.Status200OK)]
    public IActionResult GetProfileAsync()
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

    /// <summary>
    /// 获取当前用户的菜单路由
    /// </summary>
    /// <returns>菜单路由列表</returns>
    [HttpGet("routes")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult<List<MenuDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoutesAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResult.Fail("用户未登录"));
        }

        var result = await _permissionQueries.GetUserMenuTreeAsync(long.Parse(userId));
        return Ok(result);
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="command">刷新令牌命令</param>
    /// <returns>新的访问令牌和刷新令牌</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<RefreshTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenCommand command)
    {
        // 获取客户端IP地址和用户代理信息
        command.ClientIp = GetClientIpAddress();
        command.UserAgent = HttpContext.Request.Headers.UserAgent.FirstOrDefault();

        var result = await _mediator.Send(command);

        _logger.LogInformation("Token刷新成功");

        return Ok(ApiResult<RefreshTokenDto>.Ok(result, "Token刷新成功"));
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request">修改密码请求</param>
    /// <returns>修改结果</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResult.Fail("用户未登录"));
        }

        var command = new ChangePasswordCommand
        {
            UserId = long.Parse(userId),
            OldPassword = request.OldPassword,
            NewPassword = request.NewPassword
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 获取客户端IP地址
    /// 启用 UseForwardedHeaders 后，RemoteIpAddress 已由框架按转发头解析，无需手工读取原始请求头（避免伪造）
    /// </summary>
    /// <returns>IP地址</returns>
    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
