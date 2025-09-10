using Application.Commands.User;
using Repository;
using SqlSugar;

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
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var user = await context.Users.GetByIdAsync(request.Id);
                if (user == null)
                {
                    return ApiResult<bool>.Fail("用户不存在");
                }

                var success = await context.Users.DeleteAsync(user);
                context.Commit();

                _logger.LogInformation("删除用户成功，用户ID: {UserId}, 用户名: {UserName}", request.Id, user.UserName);
                return success ? ApiResult<bool>.Ok(true, "删除成功") : ApiResult<bool>.Fail("删除失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除用户失败，用户ID: {UserId}", request.Id);
            return ApiResult<bool>.Fail("删除用户失败");
        }
    }
}
