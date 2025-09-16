using Shared.Events;

namespace Application.EventHandlers;

/// <summary>
/// 用户退出登录事件处理器
/// </summary>
public class UserLogoutEventHandler : INotificationHandler<UserLogoutEvent>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UserLogoutEventHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserLogoutEventHandler(
        ISugarUnitOfWork<DBContext> unitOfWork,
        ILogger<UserLogoutEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理用户退出登录事件
    /// </summary>
    public async Task Handle(UserLogoutEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            // 更新在线用户状态为离线
            var updated = await context.Db.Updateable<OnlineUser>()
                .SetColumns(u => new OnlineUser { Status = 0, UpdateTime = DateTime.Now })
                .Where(u => u.SessionId == notification.SessionId)
                .ExecuteCommandAsync();

            context.Commit();

            if (updated > 0)
            {
                _logger.LogInformation("用户下线成功，用户: {UserName}, 会话: {SessionId}, 下线类型: {LogoutType}", 
                    notification.UserName, notification.SessionId, notification.LogoutType);
            }
            else
            {
                _logger.LogWarning("用户下线失败，未找到对应的在线会话，用户: {UserName}, 会话: {SessionId}", 
                    notification.UserName, notification.SessionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理用户退出登录事件失败，用户: {UserName}", notification.UserName);
        }
    }
}
