namespace Application.Commands.User;

/// <summary>
/// 分配用户角色命令
/// </summary>
public class AssignUserRolesCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<long> RoleIds { get; set; } = new();
} 