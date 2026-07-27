using Application.Commands.Auth;
using FluentValidation;
using Shared.Enums;

namespace Application.Validators.Auth;

/// <summary>
/// 登录命令验证器
/// </summary>
public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("登录类型不能为空")
            .IsInEnum()
            .WithMessage("登录类型无效");

        // 账号密码登录验证
        When(x => x.Type == LoginType.Password, () =>
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("用户名不能为空")
                .MaximumLength(50)
                .WithMessage("用户名长度不能超过50个字符");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("密码不能为空")
                .MinimumLength(6)
                .WithMessage("密码长度不能少于6个字符")
                .MaximumLength(20)
                .WithMessage("密码长度不能超过20个字符");
        });

        // 手机验证码登录验证
        When(x => x.Type == LoginType.SmsCode, () =>
        {
            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("手机号不能为空")
                .Matches(@"^1[3-9]\d{9}$")
                .WithMessage("手机号格式不正确");

            RuleFor(x => x.SmsCode)
                .NotEmpty()
                .WithMessage("验证码不能为空")
                .Length(6)
                .WithMessage("验证码必须是6位数字")
                .Matches(@"^\d{6}$")
                .WithMessage("验证码格式不正确");
        });

        // 钉钉登录验证
        When(x => x.Type == LoginType.DingTalk, () =>
        {
            RuleFor(x => x.DingTalkAuthCode)
                .NotEmpty()
                .WithMessage("钉钉授权码不能为空");
        });

        // 微信登录验证
        When(x => x.Type == LoginType.WeChat, () =>
        {
            RuleFor(x => x.WeChatAuthCode)
                .NotEmpty()
                .WithMessage("微信授权码不能为空");
        });
    }
}
