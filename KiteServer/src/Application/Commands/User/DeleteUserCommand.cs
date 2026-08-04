namespace Application.Commands.User;

/// <summary>
/// 删除用户命令
/// </summary>
public class DeleteUserCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>

    public DeleteUserCommand(long id)
    {
        Id = id;
    }
}
