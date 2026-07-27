namespace Api.Controllers;

/// <summary>
/// 权限管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionQueries _permissionQueries;
    private readonly ILogger<PermissionController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissionQueries">权限查询服务</param>
    /// <param name="logger">日志</param>
    public PermissionController(IPermissionQueries permissionQueries, ILogger<PermissionController> logger)
    {
        _permissionQueries = permissionQueries;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户权限信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户权限信息</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResult<UserPermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserPermissions(long userId)
    {
        var result = await _permissionQueries.GetUserPermissionsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// 获取用户菜单树
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>菜单树</returns>
    [HttpGet("user/{userId}/menus")]
    [ProducesResponseType(typeof(ApiResult<List<MenuDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserMenuTree(long userId)
    {
        var result = await _permissionQueries.GetUserMenuTreeAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// 检查用户权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permission">权限标识</param>
    /// <returns>是否有权限</returns>
    [HttpGet("user/{userId}/check")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckUserPermission(long userId, [FromQuery] string permission)
    {
        var result = await _permissionQueries.CheckUserPermissionAsync(userId, permission);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有菜单列表（用于权限配置）
    /// </summary>
    /// <returns>菜单列表</returns>
    [HttpGet("menus")]
    [ProducesResponseType(typeof(ApiResult<List<MenuDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllMenus()
    {
        var result = await _permissionQueries.GetAllMenusAsync();
        return Ok(result);
    }

    /// <summary>
    /// 根据角色ID获取菜单权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单权限列表</returns>
    [HttpGet("role/{roleId}/menus")]
    [ProducesResponseType(typeof(ApiResult<List<long>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoleMenuIds(long roleId)
    {
        var result = await _permissionQueries.GetRoleMenuIdsAsync(roleId);
        return Ok(result);
    }
} 