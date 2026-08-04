using Application.Commands.Auth;
using Application.Validators.Auth;
using FluentValidation;

namespace UnitTests.Validators.Auth;

/// <summary>
/// <see cref="ChangePasswordCommandValidator"/> 单元测试
/// </summary>
public class ChangePasswordCommandValidatorTests
{
    /// <summary>
    /// 创建合法的修改密码命令
    /// </summary>
    private static ChangePasswordCommand CreateValidCommand()
    {
        return new ChangePasswordCommand
        {
            UserId = 1,
            OldPassword = "old123456",
            NewPassword = "new654321"
        };
    }

    [Fact(DisplayName = "合法的修改密码命令应校验通过")]
    public async Task ValidateAsync_ValidCommand_Succeeds()
    {
        // 准备
        var validator = new ChangePasswordCommandValidator();
        var command = CreateValidCommand();

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "用户ID无效时校验应失败")]
    public async Task ValidateAsync_InvalidUserId_Fails()
    {
        // 准备
        var validator = new ChangePasswordCommandValidator();
        var command = CreateValidCommand();
        command.UserId = 0;

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.UserId));
    }

    [Fact(DisplayName = "旧密码为空时校验应失败")]
    public async Task ValidateAsync_EmptyOldPassword_Fails()
    {
        // 准备
        var validator = new ChangePasswordCommandValidator();
        var command = CreateValidCommand();
        command.OldPassword = string.Empty;

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.OldPassword));
    }

    [Fact(DisplayName = "新密码为空时校验应失败")]
    public async Task ValidateAsync_EmptyNewPassword_Fails()
    {
        // 准备
        var validator = new ChangePasswordCommandValidator();
        var command = CreateValidCommand();
        command.NewPassword = string.Empty;

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }

    [Theory(DisplayName = "新密码长度边界校验")]
    [InlineData("12345", false)]      // 5 位：少于 6 位下限
    [InlineData("123456", true)]      // 6 位：恰好下限
    [InlineData("12345678901234567890123456789012345678901234567890", true)]  // 50 位：恰好上限
    [InlineData("123456789012345678901234567890123456789012345678901", false)] // 51 位：超过上限
    public async Task ValidateAsync_NewPasswordLength_ChecksBounds(string newPassword, bool expected)
    {
        // 准备
        var validator = new ChangePasswordCommandValidator();
        var command = CreateValidCommand();
        command.NewPassword = newPassword;

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.Equal(expected, result.IsValid);
    }

    [Fact(DisplayName = "新密码与旧密码相同时校验应失败")]
    public async Task ValidateAsync_NewPasswordSameAsOld_Fails()
    {
        // 准备
        var validator = new ChangePasswordCommandValidator();
        var command = CreateValidCommand();
        command.NewPassword = command.OldPassword;

        // 执行
        var result = await validator.ValidateAsync(command);

        // 断言
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }
}
