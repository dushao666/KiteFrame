namespace Application.Commands.Auth;

/// <summary>
/// 修改密码命令
/// </summary>
public class ChangePasswordCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 用户ID（当前登录用户）
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 旧密码
    /// </summary>
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}
