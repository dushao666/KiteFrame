using Application.Commands.Auth;
using Application.Validators.Auth;
using FluentValidation;
using Shared.Enums;

namespace UnitTests.Validators.Auth;

/// <summary>
/// <see cref="SignInCommandValidator"/> 单元测试
/// </summary>
public class SignInCommandValidatorTests
{
    [Fact(DisplayName = "账号密码登录：用户名为空时校验应失败")]
    public async Task ValidateAsync_PasswordLoginWithEmptyUserName_Fails()
    {
        // 准备
        var validator = new SignInCommandValidator();
        var command = new SignInCommand
        {
            Type = LoginType.Password,
            UserName = string.Empty,
            Password = "123456"
        };

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(SignInCommand.UserName));
    }

    [Theory(DisplayName = "手机验证码登录：手机号格式校验")]
    [InlineData("13800138000", true)]
    [InlineData("12345", false)]
    public async Task ValidateAsync_SmsCodeLoginPhoneFormat_ChecksRegex(string phone, bool expected)
    {
        // 准备
        var validator = new SignInCommandValidator();
        var command = new SignInCommand
        {
            Type = LoginType.SmsCode,
            Phone = phone,
            SmsCode = "123456"
        };

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.Equal(expected, result.IsValid);
    }
}
