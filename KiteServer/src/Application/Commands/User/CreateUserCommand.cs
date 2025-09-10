using Shared.Models.User;

namespace Application.Commands.User;

/// <summary>
/// 创建用户命令
/// </summary>
public class CreateUserCommand : IRequest<ApiResult<long>>
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 钉钉用户ID
    /// </summary>
    public string? DingTalkId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
