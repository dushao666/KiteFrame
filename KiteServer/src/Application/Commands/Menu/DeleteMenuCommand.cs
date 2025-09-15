namespace Application.Commands.Menu;

/// <summary>
/// 删除菜单命令
/// </summary>
public class DeleteMenuCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">菜单ID</param>
    public DeleteMenuCommand(long id)
    {
        Id = id;
    }
}
