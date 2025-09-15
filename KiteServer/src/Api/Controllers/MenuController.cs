using Application.Queries.Menu.Interfaces;

namespace Api.Controllers;

/// <summary>
/// 菜单管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMenuQueries _menuQueries;
    private readonly ILogger<MenuController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mediator">MediatR</param>
    /// <param name="menuQueries">菜单查询服务</param>
    /// <param name="logger">日志</param>
    public MenuController(IMediator mediator, IMenuQueries menuQueries, ILogger<MenuController> logger)
    {
        _mediator = mediator;
        _menuQueries = menuQueries;
        _logger = logger;
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>菜单列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PagedResult<MenuDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMenus([FromQuery] GetMenusRequest request)
    {
        var result = await _menuQueries.GetMenusAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单信息</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResult<MenuDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMenu(long id)
    {
        var result = await _menuQueries.GetMenuByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 获取菜单树形结构
    /// </summary>
    /// <returns>菜单树</returns>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResult<List<MenuDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMenuTree()
    {
        var result = await _menuQueries.GetMenuTreeAsync();
        return Ok(result);
    }

    /// <summary>
    /// 根据用户ID获取用户菜单权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户菜单列表</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResult<List<MenuDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserMenus(long userId)
    {
        var result = await _menuQueries.GetUserMenusAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="request">菜单信息</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuRequest request)
    {
        var command = new CreateMenuCommand
        {
            ParentId = request.ParentId,
            MenuName = request.MenuName,
            MenuCode = request.MenuCode,
            MenuType = request.MenuType,
            Path = request.Path,
            Component = request.Component,
            Icon = request.Icon,
            Sort = request.Sort,
            IsVisible = request.IsVisible,
            IsCache = request.IsCache,
            IsFrame = request.IsFrame,
            Status = request.Status,
            Permissions = request.Permissions,
            Remark = request.Remark
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="request">菜单信息</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateMenu(long id, [FromBody] UpdateMenuRequest request)
    {
        var command = new UpdateMenuCommand
        {
            Id = id,
            ParentId = request.ParentId,
            MenuName = request.MenuName,
            MenuCode = request.MenuCode,
            MenuType = request.MenuType,
            Path = request.Path,
            Component = request.Component,
            Icon = request.Icon,
            Sort = request.Sort,
            IsVisible = request.IsVisible,
            IsCache = request.IsCache,
            IsFrame = request.IsFrame,
            Status = request.Status,
            Permissions = request.Permissions,
            Remark = request.Remark
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 删除菜单（逻辑删除）
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMenu(long id)
    {
        var command = new DeleteMenuCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
