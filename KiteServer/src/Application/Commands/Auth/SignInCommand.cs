namespace Application.Commands.Auth;

/// <summary>
/// 用户登录命令
/// </summary>
public class SignInCommand : IRequest<LoginUserDto>
{
    /// <summary>
    /// 登录类型
    /// </summary>
    public LoginType Type { get; set; } = LoginType.Password;

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 短信验证码
    /// </summary>
    public string? SmsCode { get; set; }

    /// <summary>
    /// 钉钉授权码
    /// </summary>
    public string? DingTalkAuthCode { get; set; }

    /// <summary>
    /// 微信授权码
    /// </summary>
    public string? WeChatAuthCode { get; set; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 记住我
    /// </summary>
    public bool RememberMe { get; set; } = false;

    /// <summary>
    /// 用户代理信息
    /// </summary>
    public string? UserAgent { get; set; }
}
