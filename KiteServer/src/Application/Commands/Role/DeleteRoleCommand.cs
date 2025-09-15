namespace Application.Commands.Role;

/// <summary>
/// 删除角色命令
/// </summary>
public class DeleteRoleCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">角色ID</param>
    public DeleteRoleCommand(long id)
    {
        Id = id;
    }
}
