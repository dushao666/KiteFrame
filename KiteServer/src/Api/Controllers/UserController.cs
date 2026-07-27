namespace Api.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserQueries _userQueries;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mediator">MediatR</param>
    /// <param name="userQueries">用户查询服务</param>
    /// <param name="logger">日志</param>
    public UserController(IMediator mediator, IUserQueries userQueries, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _userQueries = userQueries;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PagedResult<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
    {
        var result = await _userQueries.GetUsersAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(long id)
    {
        var result = await _userQueries.GetUserByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="request">用户信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand
        {
            UserName = request.UserName,
            Password = request.Password,
            Email = request.Email,
            Phone = request.Phone,
            RealName = request.RealName,
            DingTalkId = request.DingTalkId,
            Status = request.Status,
            Remark = request.Remark
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">用户信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand
        {
            Id = id,
            UserName = request.UserName,
            Email = request.Email,
            Phone = request.Phone,
            RealName = request.RealName,
            DingTalkId = request.DingTalkId,
            Status = request.Status,
            Remark = request.Remark
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 删除用户（逻辑删除）
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 获取用户已分配的角色ID列表
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>角色ID列表</returns>
    [HttpGet("{id}/roles")]
    [ProducesResponseType(typeof(ApiResult<List<long>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserRoles(long id)
    {
        var result = await _userQueries.GetUserRoleIdsAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 分配用户角色
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">角色分配请求</param>
    /// <returns>分配结果</returns>
    [HttpPost("{id}/roles")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignUserRoles(long id, [FromBody] AssignUserRolesRequest request)
    {
        // 确保路径参数和请求体中的用户ID一致
        request.UserId = id;

        var command = new AssignUserRolesCommand
        {
            UserId = request.UserId,
            RoleIds = request.RoleIds
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
