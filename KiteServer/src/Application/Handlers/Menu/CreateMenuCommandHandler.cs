using Application.Commands.Menu;

namespace Application.Handlers.Menu;

/// <summary>
/// 创建菜单命令处理器
/// </summary>
public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, ApiResult<long>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<CreateMenuCommandHandler> _logger;

    public CreateMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<CreateMenuCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResult<long>> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查菜单编码是否存在
                var existingMenu = await context.Menus
                    .AsQueryable()
                    .Where(x => x.MenuCode == request.MenuCode && !x.IsDeleted)
                    .FirstAsync();

                if (existingMenu != null)
                {
                    return ApiResult<long>.Fail("菜单编码已存在");
                }

                // 如果指定了父菜单，检查父菜单是否存在
                if (request.ParentId > 0)
                {
                    var parentMenu = await context.Menus
                        .AsQueryable()
                        .Where(x => x.Id == request.ParentId && !x.IsDeleted)
                        .FirstAsync();

                    if (parentMenu == null)
                    {
                        return ApiResult<long>.Fail("父菜单不存在");
                    }

                    // 检查菜单类型层级关系
                    if (parentMenu.MenuType == Shared.Enums.MenuType.Button) // 按钮类型不能有子菜单
                    {
                        return ApiResult<long>.Fail("按钮类型菜单不能有子菜单");
                    }

                    if (parentMenu.MenuType == Shared.Enums.MenuType.Menu && request.MenuType != Shared.Enums.MenuType.Button) // 菜单类型下只能有按钮
                    {
                        return ApiResult<long>.Fail("菜单类型下只能添加按钮类型的子菜单");
                    }
                }

                // 使用Mapster进行对象映射
                var menu = request.Adapt<Domain.Entities.Menu>();

                var result = await context.Menus.InsertReturnEntityAsync(menu);
                context.Commit();

                _logger.LogInformation("创建菜单成功，菜单ID: {MenuId}, 菜单名称: {MenuName}", result.Id, result.MenuName);
                return ApiResult<long>.Ok(result.Id, "创建成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建菜单失败，菜单名称: {MenuName}", request.MenuName);
            return ApiResult<long>.Fail("创建菜单失败");
        }
    }
}
