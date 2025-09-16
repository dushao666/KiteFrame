using Application.Commands.User;

namespace Application.Handlers.User;

/// <summary>
/// 分配用户角色命令处理器
/// </summary>
public class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<AssignUserRolesCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AssignUserRolesCommandHandler(
        ISugarUnitOfWork<DBContext> unitOfWork,
        ILogger<AssignUserRolesCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理分配用户角色命令
    /// </summary>
    public async Task<ApiResult<bool>> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            // 检查用户是否存在
            var userExists = await context.Db.Queryable<Domain.Entities.User>()
                .Where(u => u.Id == request.UserId && u.IsDeleted == false)
                .AnyAsync();

            if (!userExists)
            {
                return ApiResult<bool>.Fail("用户不存在");
            }

            // 检查角色是否都存在且有效
            if (request.RoleIds.Any())
            {
                var validRoleCount = await context.Db.Queryable<Domain.Entities.Role>()
                    .Where(r => request.RoleIds.Contains(r.Id) && r.Status == 1)
                    .CountAsync();

                if (validRoleCount != request.RoleIds.Count)
                {
                    return ApiResult<bool>.Fail("存在无效的角色");
                }
            }

            // 删除用户现有的角色关联
            await context.Db.Deleteable<UserRole>()
                .Where(ur => ur.UserId == request.UserId)
                .ExecuteCommandAsync();

            // 添加新的角色关联
            if (request.RoleIds.Any())
            {
                var userRoles = request.RoleIds.Select(roleId => new UserRole
                {
                    UserId = request.UserId,
                    RoleId = roleId,
                    CreateTime = DateTime.Now,
                    CreateUserId = request.UserId // 这里应该是当前操作用户的ID
                }).ToList();

                await context.Db.Insertable(userRoles).ExecuteCommandAsync();
            }

            // 提交事务
            context.Commit();
            
            _logger.LogInformation("用户 {UserId} 的角色分配已更新", request.UserId);
            return ApiResult<bool>.Ok(true, "角色分配成功");
        }
        catch (Exception ex)
        {
            // 事务会在using语句结束时自动回滚（如果没有提交）
            
            _logger.LogError(ex, "分配用户角色失败：用户ID={UserId}", request.UserId);
            return ApiResult<bool>.Fail("分配用户角色失败");
        }
    }
} 