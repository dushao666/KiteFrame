using Application.Commands.User;
namespace Application.Handlers.User;

/// <summary>
/// 删除用户命令处理器
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<DeleteUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResult<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogOperationStart("删除用户", new { UserId = request.Id });

        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var user = await context.Users.GetByIdAsync(request.Id);
                if (user == null)
                {
                    _logger.LogBusinessWarning("尝试删除不存在的用户，用户ID: {UserId}", request.Id);
                    return ApiResult<bool>.Fail("用户不存在");
                }

                // 使用框架的逻辑删除，SqlSugar的AOP会自动设置IsDeleted=true和DeleteTime
                var success = await context.Users.DeleteAsync(user);
                context.Commit();

                if (success)
                {
                    _logger.LogOperationSuccess("删除用户", new { UserId = request.Id, UserName = user.UserName });
                    return ApiResult<bool>.Ok(true, "删除成功");
                }
                else
                {
                    _logger.LogBusinessWarning("删除用户操作失败，用户ID: {UserId}, 用户名: {UserName}", request.Id, user.UserName);
                    return ApiResult<bool>.Fail("删除失败");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogOperationError("删除用户", ex, new { UserId = request.Id });
            return ApiResult<bool>.Fail("删除用户失败");
        }
    }
}
