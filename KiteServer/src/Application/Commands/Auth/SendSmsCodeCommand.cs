namespace Application.Commands.Auth;

/// <summary>
/// 发送短信验证码命令
/// </summary>
public class SendSmsCodeCommand : IRequest<bool>
{
    /// <summary>
    /// 手机号
    /// </summary>
    [Required(ErrorMessage = "手机号不能为空")]
    [Phone(ErrorMessage = "手机号格式不正确")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 验证码类型（登录、注册、找回密码等）
    /// </summary>
    public string CodeType { get; set; } = "login";

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string? ClientIp { get; set; }
}
