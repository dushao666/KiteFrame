using Application.Commands.Menu;

namespace Application.Handlers.Menu;

/// <summary>
/// 更新菜单命令处理器
/// </summary>
public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UpdateMenuCommandHandler> _logger;

    public UpdateMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<UpdateMenuCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResult<bool>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查菜单是否存在
                var existingMenu = await context.Menus
                    .AsQueryable()
                    .Where(x => x.Id == request.Id && !x.IsDeleted)
                    .FirstAsync();

                if (existingMenu == null)
                {
                    return ApiResult<bool>.Fail("菜单不存在");
                }

                // 检查菜单编码是否被其他菜单使用
                var duplicateMenu = await context.Menus
                    .AsQueryable()
                    .Where(x => x.MenuCode == request.MenuCode && x.Id != request.Id && !x.IsDeleted)
                    .FirstAsync();

                if (duplicateMenu != null)
                {
                    return ApiResult<bool>.Fail("菜单编码已存在");
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
                        return ApiResult<bool>.Fail("父菜单不存在");
                    }

                    // 不能将菜单设置为自己的子菜单
                    if (request.ParentId == request.Id)
                    {
                        return ApiResult<bool>.Fail("不能将菜单设置为自己的子菜单");
                    }

                    // 检查是否会形成循环引用
                    if (await IsCircularReference(context, request.Id, request.ParentId))
                    {
                        return ApiResult<bool>.Fail("不能设置为子菜单的父菜单，会形成循环引用");
                    }

                    // 检查菜单类型层级关系
                    if (parentMenu.MenuType == Shared.Enums.MenuType.Button) // 按钮类型不能有子菜单
                    {
                        return ApiResult<bool>.Fail("按钮类型菜单不能有子菜单");
                    }

                    if (parentMenu.MenuType == Shared.Enums.MenuType.Menu && request.MenuType != Shared.Enums.MenuType.Button) // 菜单类型下只能有按钮
                    {
                        return ApiResult<bool>.Fail("菜单类型下只能添加按钮类型的子菜单");
                    }
                }

                // 使用Mapster进行对象映射
                request.Adapt(existingMenu);
                existingMenu.UpdateTime = DateTime.Now;

                await context.Menus.UpdateAsync(existingMenu);
                context.Commit();

                _logger.LogInformation("更新菜单成功，菜单ID: {MenuId}, 菜单名称: {MenuName}", request.Id, request.MenuName);
                return ApiResult<bool>.Ok(true, "更新成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新菜单失败，菜单ID: {MenuId}", request.Id);
            return ApiResult<bool>.Fail("更新菜单失败");
        }
    }

    /// <summary>
    /// 检查是否会形成循环引用
    /// </summary>
    /// <param name="context">数据库上下文</param>
    /// <param name="menuId">当前菜单ID</param>
    /// <param name="parentId">父菜单ID</param>
    /// <returns>是否会形成循环引用</returns>
    private async Task<bool> IsCircularReference(DBContext context, long menuId, long parentId)
    {
        var currentParentId = parentId;
        while (currentParentId > 0)
        {
            if (currentParentId == menuId)
            {
                return true;
            }

            var parentMenu = await context.Menus
                .AsQueryable()
                .Where(x => x.Id == currentParentId && !x.IsDeleted)
                .FirstAsync();

            if (parentMenu == null)
            {
                break;
            }

            currentParentId = parentMenu.ParentId;
        }

        return false;
    }
}
