
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
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
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
            // 获取客户端IP地址
            command.ClientIp = GetClientIpAddress();

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

            // TODO: 将当前用户的Token加入黑名单
            // TODO: 清除缓存中的用户信息

            _logger.LogInformation("用户 {UserName} (ID: {UserId}) 登出成功", userName, userId);

            return Ok(ApiResult.Ok("登出成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出过程中发生错误");
            return StatusCode(500, ApiResult.Fail("登出失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的访问令牌</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<LoginUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken)
    {
        try
        {
            // TODO: 实现刷新令牌逻辑
            // 1. 验证刷新令牌是否有效
            // 2. 根据刷新令牌获取用户信息
            // 3. 生成新的访问令牌和刷新令牌
            // 4. 更新缓存中的令牌信息

            return BadRequest(ApiResult.Fail("刷新令牌功能暂未实现"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌过程中发生错误");
            return StatusCode(500, ApiResult.Fail("刷新令牌失败，请重新登录"));
        }
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet("userinfo")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult<LoginUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserInfoAsync()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResult.Fail("用户未登录"));
            }

            // TODO: 从数据库或缓存中获取最新的用户信息
            var userInfo = new LoginUserDto
            {
                UserId = long.Parse(userId),
                UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                RealName = User.FindFirst("RealName")?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
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
    /// 修改密码
    /// </summary>
    /// <param name="oldPassword">旧密码</param>
    /// <param name="newPassword">新密码</param>
    /// <returns>修改结果</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        try
        {
            // TODO: 实现修改密码逻辑
            // 1. 验证旧密码是否正确
            // 2. 验证新密码格式
            // 3. 更新用户密码
            // 4. 记录操作日志

            return BadRequest(ApiResult.Fail("修改密码功能暂未实现"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密码过程中发生错误");
            return StatusCode(500, ApiResult.Fail("修改密码失败"));
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
        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }
        else if (HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
        {
            ipAddress = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        return ipAddress ?? "Unknown";
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
