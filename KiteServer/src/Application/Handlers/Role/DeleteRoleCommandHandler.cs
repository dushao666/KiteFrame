using Application.Commands.Role;

namespace Application.Handlers.Role;

/// <summary>
/// 删除角色命令处理器
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    public DeleteRoleCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<DeleteRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResult<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogOperationStart("删除角色", new { RoleId = request.Id });

        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var role = await context.Roles.GetByIdAsync(request.Id);
                if (role == null)
                {
                    _logger.LogBusinessWarning("尝试删除不存在的角色，角色ID: {RoleId}", request.Id);
                    return ApiResult<bool>.Fail("角色不存在");
                }

                // 检查是否有用户关联此角色
                var hasUsers = await context.UserRoles
                    .AsQueryable()
                    .Where(x => x.RoleId == request.Id)
                    .AnyAsync();

                if (hasUsers)
                {
                    _logger.LogBusinessWarning("角色已被用户关联，无法删除，角色ID: {RoleId}, 角色名称: {RoleName}", request.Id, role.RoleName);
                    return ApiResult<bool>.Fail("角色已被用户关联，无法删除");
                }

                // 检查是否有菜单权限关联此角色
                var hasMenus = await context.RoleMenus
                    .AsQueryable()
                    .Where(x => x.RoleId == request.Id)
                    .AnyAsync();

                if (hasMenus)
                {
                    // 先删除角色菜单关联
                    await context.RoleMenus
                        .AsDeleteable()
                        .Where(x => x.RoleId == request.Id)
                        .ExecuteCommandAsync();
                }

                // 使用框架的逻辑删除，SqlSugar的AOP会自动设置IsDeleted=true和DeleteTime
                var success = await context.Roles.DeleteAsync(role);
                context.Commit();

                if (success)
                {
                    _logger.LogOperationSuccess("删除角色", new { RoleId = request.Id, RoleName = role.RoleName });
                    return ApiResult<bool>.Ok(true, "删除成功");
                }
                else
                {
                    _logger.LogBusinessWarning("删除角色操作失败，角色ID: {RoleId}, 角色名称: {RoleName}", request.Id, role.RoleName);
                    return ApiResult<bool>.Fail("删除失败");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除角色失败，角色ID: {RoleId}", request.Id);
            return ApiResult<bool>.Fail("删除角色失败");
        }
    }
}
