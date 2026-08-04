namespace Application.Validators.Auth;

/// <summary>
/// 修改密码命令验证器
/// </summary>
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("用户ID无效");

        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("旧密码不能为空");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("新密码不能为空")
            .MinimumLength(6)
            .WithMessage("新密码长度不能少于6个字符")
            .MaximumLength(50)
            .WithMessage("新密码长度不能超过50个字符")
            .NotEqual(x => x.OldPassword)
            .WithMessage("新密码不能与旧密码相同");
    }
}
