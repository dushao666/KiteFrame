using Application.Commands.User;
using Repository;
using SqlSugar;

namespace Application.Handlers.User;

/// <summary>
/// 更新用户命令处理器
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<UpdateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResult<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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

                // 检查用户名是否存在（排除当前用户）
                var existingUser = await context.Users
                    .AsQueryable()
                    .Where(x => x.UserName == request.UserName && x.Id != request.Id)
                    .FirstAsync();

                if (existingUser != null)
                {
                    return ApiResult<bool>.Fail("用户名已存在");
                }

                user.UserName = request.UserName;
                user.Email = request.Email;
                user.Phone = request.Phone;
                user.RealName = request.RealName;
                user.DingTalkId = request.DingTalkId;
                user.Status = request.Status;
                user.Remark = request.Remark;

                var success = await context.Users.UpdateAsync(user);
                context.Commit();

                _logger.LogInformation("更新用户成功，用户ID: {UserId}, 用户名: {UserName}", request.Id, request.UserName);
                return success ? ApiResult<bool>.Ok(true, "更新成功") : ApiResult<bool>.Fail("更新失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用户失败，用户ID: {UserId}", request.Id);
            return ApiResult<bool>.Fail("更新用户失败");
        }
    }
}
