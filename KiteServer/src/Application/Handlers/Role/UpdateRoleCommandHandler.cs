using Application.Commands.Role;
using Application.Queries.Role.Interfaces;

namespace Application.Handlers.Role;

/// <summary>
/// 更新角色命令处理器
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;
    private readonly IRoleQueries _roleQueries;

    public UpdateRoleCommandHandler(
        ISugarUnitOfWork<DBContext> unitOfWork, 
        ILogger<UpdateRoleCommandHandler> logger,
        IRoleQueries roleQueries)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _roleQueries = roleQueries;
    }

    public async Task<ApiResult<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogOperationStart("更新角色", new { RoleId = request.Id, RoleName = request.RoleName });

        try
        {
            // 检查角色编码是否已被其他角色使用
            var codeExistsResult = await _roleQueries.CheckRoleCodeExistsAsync(request.RoleCode, request.Id);
            if (!codeExistsResult.Success)
            {
                return ApiResult<bool>.Fail("检查角色编码失败");
            }

            if (codeExistsResult.Data)
            {
                return ApiResult<bool>.Fail($"角色编码 '{request.RoleCode}' 已被其他角色使用");
            }

            using var context = _unitOfWork.CreateContext(true);

            // 获取现有角色
            var existingRole = await context.Roles.GetByIdAsync(request.Id);
            if (existingRole == null)
            {
                return ApiResult<bool>.Fail("角色不存在");
            }

            // 更新角色信息
            existingRole.RoleName = request.RoleName;
            existingRole.RoleCode = request.RoleCode;
            existingRole.Sort = request.Sort;
            existingRole.Status = request.Status;
            existingRole.DataScope = request.DataScope;
            existingRole.Remark = request.Remark;

            var success = await context.Roles.UpdateAsync(existingRole);
            if (success)
            {
                context.Commit();
                _logger.LogInformation("更新角色成功，角色ID: {RoleId}", request.Id);
                return ApiResult<bool>.Ok(true, "角色更新成功");
            }

            return ApiResult<bool>.Fail("角色更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新角色失败，角色ID: {RoleId}", request.Id);
            return ApiResult<bool>.Fail("角色更新失败");
        }
    }
}
