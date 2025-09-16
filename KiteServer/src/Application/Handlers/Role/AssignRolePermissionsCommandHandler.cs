using Application.Commands.Role;

namespace Application.Handlers.Role;

/// <summary>
/// 分配角色权限命令处理器
/// </summary>
public class AssignRolePermissionsCommandHandler : IRequestHandler<AssignRolePermissionsCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<AssignRolePermissionsCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AssignRolePermissionsCommandHandler(
        ISugarUnitOfWork<DBContext> unitOfWork,
        ILogger<AssignRolePermissionsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理分配角色权限命令
    /// </summary>
    public async Task<ApiResult<bool>> Handle(AssignRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            // 检查角色是否存在
            var roleExists = await context.Db.Queryable<Domain.Entities.Role>()
                .Where(r => r.Id == request.RoleId && r.Status == 1)
                .AnyAsync();

            if (!roleExists)
            {
                return ApiResult<bool>.Fail("角色不存在或已禁用");
            }

            // 检查菜单是否都存在且有效
            if (request.MenuIds.Any())
            {
                var validMenuCount = await context.Db.Queryable<Domain.Entities.Menu>()
                    .Where(m => request.MenuIds.Contains(m.Id) && m.Status == 1)
                    .CountAsync();

                if (validMenuCount != request.MenuIds.Count)
                {
                    return ApiResult<bool>.Fail("存在无效的菜单");
                }
            }

            // 删除角色现有的菜单权限关联
            await context.Db.Deleteable<RoleMenu>()
                .Where(rm => rm.RoleId == request.RoleId)
                .ExecuteCommandAsync();

            // 添加新的菜单权限关联
            if (request.MenuIds.Any())
            {
                var roleMenus = request.MenuIds.Select(menuId => new RoleMenu
                {
                    RoleId = request.RoleId,
                    MenuId = menuId,
                    CreateTime = DateTime.Now,
                    CreateUserId = request.RoleId // 这里应该是当前操作用户的ID
                }).ToList();

                await context.Db.Insertable(roleMenus).ExecuteCommandAsync();
            }

            // 提交事务
            context.Commit();
            
            _logger.LogInformation("角色 {RoleId} 的权限分配已更新", request.RoleId);
            return ApiResult<bool>.Ok(true, "权限分配成功");
        }
        catch (Exception ex)
        {
            // 事务会在using语句结束时自动回滚（如果没有提交）
            _logger.LogError(ex, "分配角色权限失败：角色ID={RoleId}", request.RoleId);
            return ApiResult<bool>.Fail("分配角色权限失败");
        }
    }
} 