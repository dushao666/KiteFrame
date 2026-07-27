using Application.Commands.Role;
using Application.Queries.Role.Interfaces;

namespace Application.Handlers.Role;

/// <summary>
/// 创建角色命令处理器
/// </summary>
public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResult<long>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<CreateRoleCommandHandler> _logger;
    private readonly IRoleQueries _roleQueries;

    public CreateRoleCommandHandler(
        ISugarUnitOfWork<DBContext> unitOfWork, 
        ILogger<CreateRoleCommandHandler> logger,
        IRoleQueries roleQueries)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _roleQueries = roleQueries;
    }

    public async Task<ApiResult<long>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogOperationStart("创建角色", new { RoleName = request.RoleName, RoleCode = request.RoleCode });

        try
        {
            // 检查角色编码是否已存在
            var existsResult = await _roleQueries.CheckRoleCodeExistsAsync(request.RoleCode);
            if (!existsResult.Success)
            {
                return ApiResult<long>.Fail("检查角色编码失败");
            }

            if (existsResult.Data)
            {
                return ApiResult<long>.Fail($"角色编码 '{request.RoleCode}' 已存在");
            }

            using var context = _unitOfWork.CreateContext(true);

            var role = new Domain.Entities.Role
            {
                RoleName = request.RoleName,
                RoleCode = request.RoleCode,
                Sort = request.Sort,
                Status = request.Status,
                DataScope = request.DataScope,
                Remark = request.Remark
            };

            var result = await context.Roles.InsertReturnEntityAsync(role);
            context.Commit();

            _logger.LogInformation("创建角色成功，角色ID: {RoleId}", result.Id);
            return ApiResult<long>.Ok(result.Id, "角色创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建角色失败");
            return ApiResult<long>.Fail("角色创建失败");
        }
    }
}
