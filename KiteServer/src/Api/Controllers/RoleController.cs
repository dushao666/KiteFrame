namespace Api.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleQueries _roleQueries;
    private readonly IMediator _mediator;
    private readonly ILogger<RoleController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleQueries">角色查询服务</param>
    /// <param name="mediator">中介者</param>
    /// <param name="logger">日志</param>
    public RoleController(IRoleQueries roleQueries, IMediator mediator, ILogger<RoleController> logger)
    {
        _roleQueries = roleQueries;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 分页获取角色列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>角色列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PagedResult<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoles([FromQuery] GetRolesRequest request)
    {
        var result = await _roleQueries.GetRolesAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResult<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoleById(long id)
    {
        var result = await _roleQueries.GetRoleByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有启用的角色列表（用于下拉选择）
    /// </summary>
    /// <returns>角色列表</returns>
    [HttpGet("enabled")]
    [ProducesResponseType(typeof(ApiResult<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEnabledRoles()
    {
        var result = await _roleQueries.GetEnabledRolesAsync();
        return Ok(result);
    }

    /// <summary>
    /// 检查角色编码是否存在
    /// </summary>
    /// <param name="roleCode">角色编码</param>
    /// <param name="excludeId">排除的角色ID</param>
    /// <returns>是否存在</returns>
    [HttpGet("check-code")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckRoleCodeExists([FromQuery] string roleCode, [FromQuery] long? excludeId = null)
    {
        var result = await _roleQueries.CheckRoleCodeExistsAsync(roleCode, excludeId);
        return Ok(result);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>角色ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var command = new CreateRoleCommand
        {
            RoleName = request.RoleName,
            RoleCode = request.RoleCode,
            Sort = request.Sort,
            Status = request.Status,
            DataScope = (DataScope)request.DataScope,
            Remark = request.Remark
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleRequest request)
    {
        var command = new UpdateRoleCommand
        {
            Id = id,
            RoleName = request.RoleName,
            RoleCode = request.RoleCode,
            Sort = request.Sort,
            Status = request.Status,
            DataScope = (DataScope)request.DataScope,
            Remark = request.Remark
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>是否成功</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRole(long id)
    {
        var command = new DeleteRoleCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 分配角色权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="request">权限分配请求</param>
    /// <returns>分配结果</returns>
    [HttpPost("{roleId}/permissions")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignRolePermissions(long roleId, [FromBody] AssignRolePermissionsRequest request)
    {
        var command = new AssignRolePermissionsCommand
        {
            RoleId = roleId,
            MenuIds = request.MenuIds
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
