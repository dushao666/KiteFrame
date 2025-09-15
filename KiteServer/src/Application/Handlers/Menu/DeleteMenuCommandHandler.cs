using Application.Commands.Menu;

namespace Application.Handlers.Menu;

/// <summary>
/// 删除菜单命令处理器
/// </summary>
public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<DeleteMenuCommandHandler> _logger;

    public DeleteMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<DeleteMenuCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResult<bool>> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
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

                // 检查是否有子菜单
                var hasChildren = await context.Menus
                    .AsQueryable()
                    .Where(x => x.ParentId == request.Id && !x.IsDeleted)
                    .AnyAsync();

                if (hasChildren)
                {
                    return ApiResult<bool>.Fail("该菜单下存在子菜单，无法删除");
                }

                // 检查是否有角色关联
                var hasRoleAssociation = await context.RoleMenus
                    .AsQueryable()
                    .Where(x => x.MenuId == request.Id)
                    .AnyAsync();

                if (hasRoleAssociation)
                {
                    return ApiResult<bool>.Fail("该菜单已分配给角色，无法删除");
                }

                // 逻辑删除
                existingMenu.IsDeleted = true;
                existingMenu.DeleteTime = DateTime.Now;
                // 注意：这里应该设置DeleteUserId，但需要从当前用户上下文获取

                await context.Menus.UpdateAsync(existingMenu);
                context.Commit();

                _logger.LogInformation("删除菜单成功，菜单ID: {MenuId}, 菜单名称: {MenuName}", request.Id, existingMenu.MenuName);
                return ApiResult<bool>.Ok(true, "删除成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除菜单失败，菜单ID: {MenuId}", request.Id);
            return ApiResult<bool>.Fail("删除菜单失败");
        }
    }
}
