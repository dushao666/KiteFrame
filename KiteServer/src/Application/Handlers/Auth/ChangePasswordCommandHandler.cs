namespace Application.Handlers.Auth;

/// <summary>
/// 修改密码命令处理器
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResult<bool>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元</param>
    /// <param name="logger">日志</param>
    public ChangePasswordCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<ChangePasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理修改密码
    /// </summary>
    /// <param name="request">修改密码命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>修改结果</returns>
    public async Task<ApiResult<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext();

            // 查找用户
            var user = await context.Users.GetFirstAsync(x => x.Id == request.UserId);
            if (user == null)
            {
                return ApiResult<bool>.Fail("用户不存在");
            }

            // 验证旧密码（兼容历史遗留的无盐 SHA512 哈希）
            var verifyResult = PasswordHasher.VerifyPassword(request.OldPassword, user.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return ApiResult<bool>.Fail("旧密码不正确");
            }

            // 使用当前算法重新哈希新密码（无论旧密码是哪种算法，改密后统一升级）
            user.Password = PasswordHasher.HashPassword(request.NewPassword);

            await context.Users.UpdateAsync(user);
            context.Commit();

            _logger.LogInformation("用户 {UserId} 修改密码成功", request.UserId);
            return ApiResult<bool>.Ok(true, "密码修改成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用户 {UserId} 修改密码失败", request.UserId);
            return ApiResult<bool>.Fail("密码修改失败");
        }
    }
}
