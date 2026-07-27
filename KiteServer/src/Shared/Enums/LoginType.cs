using System.ComponentModel;

namespace Shared.Enums;

/// <summary>
/// 登录类型枚举
/// </summary>
public enum LoginType
{
    /// <summary>
    /// 账号密码登录
    /// </summary>
    [Description("账号密码登录")]
    Password = 1,

    /// <summary>
    /// 钉钉登录
    /// </summary>
    [Description("钉钉登录")]
    DingTalk = 2,

    /// <summary>
    /// 微信登录
    /// </summary>
    [Description("微信登录")]
    WeChat = 3,

    /// <summary>
    /// 手机验证码登录
    /// </summary>
    [Description("手机验证码登录")]
    SmsCode = 4
}
